﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UChainDB.Example.Chain.Entity;

namespace UChainDB.Example.Chain.Network.InMemory
{
    public class InMemoryClientServerCenter
    {
        private int number = 0;

        private ConcurrentDictionary<string, InMemoryListener> dicServers = new ConcurrentDictionary<string, InMemoryListener>();
        private ConcurrentDictionary<string, List<InMemoryClientBase>> dicPeers = new ConcurrentDictionary<string, List<InMemoryClientBase>>();

        public InMemoryClientServerCenter()
        {
        }

        public NodeOptions NodeOptions { get => new NodeOptions { WellKnownNodes = this.ProduceWellKnownNodes() }; }

        public string[] ProduceWellKnownNodes() => Enumerable.Range(0, this.number).Select(_ => _.ToString()).ToArray();

        public (IListener listener, IPeerFactory clientFactory) Produce()
        {
            var listener = this.ProduceListener();
            var peerFactory = this.ProducePeerFactory(listener);
            return (listener, peerFactory);
        }

        internal void AddPeer(InMemoryClientBase client)
        {
            var key = client.TargetAddress;
            if (this.dicPeers.ContainsKey(key))
            {
                this.dicPeers[key].Add(client);
            }
            else
            {
                this.dicPeers[key] = new List<InMemoryClientBase>(new[] { client });
            }
        }

        internal void RemovePeer(InMemoryClientBase client)
        {
            var key = client.TargetAddress;
            if (this.dicPeers.ContainsKey(key))
            {
                this.dicPeers[key].Remove(client);
            }
            else
            {
                // nothing to remove
            }
        }

        internal bool Connect(string baseAddress, ActiveInMemoryClient client)
        {
            return this.dicServers[baseAddress].Connect(client);
        }

        private InMemoryListener ProduceListener()
        {
            var address = this.number.ToString();
            var server = new InMemoryListener(this, address);
            this.dicServers[address] = server;
            this.number++;
            return server;
        }

        private IPeerFactory ProducePeerFactory(InMemoryListener server)
        {
            return new InMemoryClientFactory(this, server);
        }
    }
}