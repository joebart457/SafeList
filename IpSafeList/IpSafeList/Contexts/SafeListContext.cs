using IpSafeList.Managers;
using IpSafeList.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpSafeList.Contexts
{
    public static class SafeListContext
    {
        private static ConnectionManager? _connectionManager;
        public static ConnectionManager Connection { get { return CreateContext(); } }

        private static ConnectionManager CreateContext(string dbPath = "")
        {
            if (_connectionManager != null) return _connectionManager;
            _connectionManager = new ConnectionManager(dbPath);
            _connectionManager.Register<IdentityEntity>();
            _connectionManager.Register<IpEntryEntity>();
            return _connectionManager;
        }
    }
}
