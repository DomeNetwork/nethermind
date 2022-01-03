using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethermind.Core.Crypto;
using Nethermind.Serialization.Rlp;
using Nethermind.State.Proofs;
using Nethermind.Trie;
using Nethermind.Trie.Pruning;

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

        public void RemoveMiddleChildren(ITrieNodeResolver resolver, byte[][] leftProof, byte[][] rightProof)
        {
            for (int i = leftProof.Length - 1; i > 0; i--)
            {
                TrieNode parentNode = new(NodeType.Unknown, leftProof[i - 1]);
                parentNode.ResolveNode(NullTrieNodeResolver.Instance);

                Keccak proofHash = Keccak.Compute(leftProof[i]);
                

                //if (proofHash != root)
                //{
                //    if (i > 0)
                //    {
                //        if (!new Rlp(proofs[i - 1]).ToString(false).Contains(proofHash.ToString(false)))
                //        {
                //            return (false, provedValues);
                //        }
                //    }
                //    else
                //    {
                //        return (false, provedValues);
                //    }
                //}
                //else
                //{
                //    TrieNode trieNode = new(NodeType.Unknown, proofs[leafIndex]);
                //    trieNode.ResolveNode(null);
                //    provedValues.Add(trieNode.Value);

                //    leafIndex = i - 1;
                //}
            }

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
