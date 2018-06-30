﻿using System;
using System.Linq;
using UChainDB.Example.BlockChain.Chain;
using UChainDB.Example.BlockChain.Core;

namespace UChainDB.Example.BlockChain
{
    class Program
    {
        const string myName = "Icer(Miner)";
        const string AliceName = "Alice";
        const string BobName = "Bob";
        static void Main(string[] args)
        {
            Console.WriteLine($"Press any key to stop....");
            var engine = new Engine(myName);
            Console.WriteLine($"Genesis Block: {Core.BlockChain.GenesisBlock}");
            engine.OnNewBlockCreated += Engine_OnNewBlockCreated;

            Console.ReadKey();
            engine.Dispose();
            Console.WriteLine($"Stopped, press any key to exit....");
            Console.ReadKey();
        }

        private static void Engine_OnNewBlockCreated(object sender, Block block)
        {
            var engine = sender as Engine;
            var height = engine.BlockChain.Height;
            Console.WriteLine($"New block created at height[{height:0000}]: {engine.BlockChain.Tail}");

            if (height == 2)
            {
                var utxo = engine.BlockChain.Tail.Transactions.First();
                SendMoney(engine, utxo, AliceName, 50);
            }
            else if (height == 3)
            {
                var utxo = engine.BlockChain.Tail.Transactions
                    .First(txs => txs.OutputOwners.Any(_ => _.Owner == AliceName));
                SendMoney(engine, utxo, BobName, 50);
            }
        }

        private static void SendMoney(Engine engine, Transaction utxo, string receiver, int value)
        {
            SendMoney(engine, utxo, new TransactionOutput { Owner = receiver, Value = value });
        }

        private static void SendMoney(Engine engine, Transaction utxo, params TransactionOutput[] outputs)
        {
            engine.AttachTransaction(new Transaction
            {
                InputTransactions = new[] { utxo.Hash },
                OutputOwners = outputs,
            });
        }
    }
}
