using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethermind.Core.Crypto;
using Nethermind.Serialization.Rlp;
using Nethermind.State.Proofs;

namespace Nethermind.State.SnapSync
{
    public class SnapProvider
    {
        private readonly StateTree _tree;
        private SortedSet<Keccak> _sortedAddressHashes = new();

        public Keccak RootHash => _tree.RootHash;

        public SnapProvider(StateTree tree)
        {
            _tree = tree;
        }

        public bool AddAccountRange(long blockNumber, Keccak expectedRootHash, Keccak startingHash, AccountWithAddressHash[] accounts, byte[][] proofs)
        {
            (bool proved , _) = ProofVerifier.VerifyMultipleProofs(proofs, expectedRootHash);

            if(!proved)
            {
                return false;
            }

            for (int i = 0; i < accounts.Length; i++)
            {
                AccountWithAddressHash account = accounts[i];

                Rlp accountRlp = _tree.Set(account.AddressHash, account.Account);
                _sortedAddressHashes.Add(account.AddressHash);
            }

            _tree.Commit(blockNumber);

            return true;
        }

        //public AccountRange GetNextAccountRange()
        //{
        //    if (_sortedAddressHashes.Count == 0)
        //    {
        //        return new(SyncRootHash, Keccak.Zero, Keccak.MaxValue);
        //    }

        //    Keccak last = _sortedAddressHashes.Last();  // TODO: calculate next Keccak after the last account (+1)

        //    return new(SyncRootHash, last, Keccak.MaxValue);    // TODO: calculate LimitHash instead MaxValue
        //}
    }
}
