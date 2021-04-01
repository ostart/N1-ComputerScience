using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace N1_ComputerScience
{
    public class Blockchain
    {
        public List<Block> Blocks {get;set;}

        private int NumberOfZeros {get;set;}

        public Blockchain(int numberOfZeros)
        {
            NumberOfZeros = numberOfZeros;
            Blocks = new List<Block> { new Block{ Hash = "00000", DateTime = DateTime.UtcNow } };
        }

        public string Hash(Block block)
        {
            var hashWithNonce = block.PreviousBlockHash.GetHashCode() ^ block.DateTime.GetHashCode() ^ block.Index.GetHashCode() ^ block.Data.GetHashCode() ^ block.Nonce.GetHashCode();
            return hashWithNonce.ToString();
        }

        public string CalculateHash(Block block)
        {
            var counter = 0;
            while(true)
            {
                block.Nonce = counter;
                var hash = Hash(block);
                var match = Regex.Match(hash, "0{" + NumberOfZeros + "}$");
                if (match.Success) return hash;
                counter += 1;
            }
        }

        public void AddBlock(string data)
        {
            var previousBlock = Blocks[Blocks.Count - 1];
            var block = new Block { PreviousBlockHash = previousBlock.Hash, Data = data, DateTime = DateTime.UtcNow, Index = previousBlock.Index + 1 };
            var hash = CalculateHash(block);
            block.Hash = hash;
            Blocks.Add(block);
        }

        public Block GetBlock(string hash)
        {
            return Blocks.SingleOrDefault(x => x.Hash == hash);
        }

        public bool IsCorrectChain()
        {
            return Blocks.Skip(1).All(x => Hash(x) == x.Hash);
        }
    }

    public class Block
    {
        public string Hash {get;set;}
        public string PreviousBlockHash {get;set;}
        public int Index {get;set;}
        public DateTime DateTime {get;set;}
        public string Data {get;set;}
        public int Nonce {get;set;}
    }
}