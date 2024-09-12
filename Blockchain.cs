using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Blockchain
{
    [Serializable]
    public class Transaction
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public float Amount { get; set; }
    }

    [Serializable]
    public class BlockHeader
    {
        public long Timestamp { get; set; }
        public uint Nonce { get; set; }
        public string PreviousHash { get; set; }
        public string MerkleRoot { get; set; }
        public uint Difficulty { get; set; }
    }

    [Serializable]
    public class Block
    {
        public BlockHeader Header { get; set; }
        public uint TransactionCount { get; set; }
        public List<Transaction> Transactions { get; set; }
    }

    public class Chain
    {
        private List<Block> _chain;
        private List<Transaction> _currentTransactions;
        private uint _difficulty;
        private string _minerAddress;
        private float _reward;
        
        public Chain(string minerAddress, uint difficulty) {
            _chain = new List<Block>();
            _currentTransactions = new List<Transaction>();
            _difficulty = difficulty;
            _minerAddress = minerAddress;
            _reward = 100.0f;

            GenerateNewBlock();
        }
        
        public bool GenerateNewBlock()
        {
            var header = new BlockHeader
            {
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Nonce = 0,
                PreviousHash = LastHash(),
                MerkleRoot = string.Empty,
                Difficulty = _difficulty
            };

            var rewardTransaction = new Transaction
            {
                Sender = "Root",
                Receiver = _minerAddress,
                Amount = _reward
            };

            var block = new Block
            {
                Header = header,
                Transactions = new List<Transaction> { rewardTransaction }
            };
            
            block.Transactions.AddRange(_currentTransactions);
            block.TransactionCount = (uint)block.Transactions.Count;
            block.Header.MerkleRoot = GetMerkleRoot(block.Transactions);
            ProofOfWork(block.Header);
            
            Console.WriteLine(JsonConvert.SerializeObject(block, Formatting.Indented));
            _chain.Add(block);
            _currentTransactions.Clear();
            return true;
        }

        public bool NewTransaction(string sender, string receiver, float amount)
        {
            _currentTransactions.Add(new Transaction
            {
                Sender = sender,
                Receiver = receiver,
                Amount = amount
            });
            return true;
        }

        public string LastHash()
        {
            if (_chain.Count == 0) return new string('0', 64);
            return Hash(_chain.Last().Header);
        }

        public bool UpdateDifficulty(uint difficulty)
        {
            _difficulty = difficulty;
            return true;
        }

        public bool UpdateReward(float reward)
        {
            _reward = reward;
            return true;
        }

        private static string GetMerkleRoot(List<Transaction> transactions)
        {
            var merkle = transactions.Select(t => Hash(t)).ToList();
            
            if (merkle.Count % 2 == 1)
                merkle.Add(merkle.Last());
            
            while (merkle.Count > 1)
            {
                var newMerkle = new List<string>();

                for (int i = 0; i < merkle.Count; i += 2)
                {
                    newMerkle.Add(Hash(merkle[i] + merkle[i + 1]));
                }

                merkle = newMerkle;
            }
            
            return merkle.First();
        }
        
        public static void ProofOfWork(BlockHeader header)
        {
            while (true)
            {
                var hash = Hash(header);
                if (hash.Substring(0, (int)header.Difficulty) == new string('0', (int)header.Difficulty))
                {
                    Console.WriteLine($"Block hash: {hash}");
                    break;
                }
                header.Nonce++;
            }
        }
        
        public static string Hash<T>(T item)
        {
            string input = JsonConvert.SerializeObject(item);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}