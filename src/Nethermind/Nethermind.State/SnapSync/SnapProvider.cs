using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethermind.Core;
using Nethermind.Core.Crypto;
using Nethermind.Logging;
using Nethermind.Serialization.Rlp;
using Nethermind.State.Proofs;
using Nethermind.Trie;
using Nethermind.Trie.Pruning;

namespace Nethermind.State.SnapSync
{
    public class SnapProvider
    {
        //private readonly StateTree _tree;
        private readonly TrieStore _store;
        private SortedSet<Keccak> _sortedAddressHashes = new();

        public SnapProvider(StateTree tree, TrieStore store)
        {
            //_tree = tree;
            _store = store;
        }

        public Keccak? AddAccountRange(long blockNumber, Keccak expectedRootHash, Keccak startingHash, AccountWithAddressHash[] accounts, byte[][] proofs)
        {
            // TODO: Check the accounts boundaries and sorting

            (bool proved, _) = ProofVerifier.VerifyMultipleProofs(proofs, expectedRootHash);

            if (!proved)
            {
                //TODO: log incorrect proofs
                return null;
            }

            IBatch batch = _store.GetOrStartNewBatch();

            StateTree tree = new StateTree(_store, LimboLogs.Instance);

            for (int i = 0; i < proofs.Length; i++)
            {
                byte[] proof = proofs[i];

                var node = new TrieNode(NodeType.Unknown, proof);
                node.ResolveNode(_store);
                node.ResolveKey(_store, i == 0);

                if (!node.IsLeaf)
                {
                    if (i == 0)
                    {
                        tree.RootRef = node;
                    }
                    else
                    {
                        batch[node.Keccak!.Bytes] = proof;
                    }
                }
            }

            foreach (var account in accounts)
            {
                tree.Set(account.AddressHash, account.Account, true);
            }

            //for (int i = 0; i < accounts.Length; i++)
            //{
            //    AccountWithAddressHash account = accounts[i];

            //    Rlp accountRlp = tree.Set(account.AddressHash, account.Account);
            //    _sortedAddressHashes.Add(account.AddressHash);
            //}

            tree.Commit(blockNumber);

            return tree.RootHash;
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
