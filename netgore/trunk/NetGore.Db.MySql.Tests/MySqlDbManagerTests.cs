﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace NetGore.Db.MySql.Tests
{
    [TestFixture]
    public class MySqlDbManagerTests
    {
        [Test]
        public void CleanupConnectionsTest()
        {
            var manager = TestSettings.CreateDbManager();
            var stack = new Stack<IPoolableDbConnection>();

            Assert.AreEqual(0, manager.ConnectionPool.Count);
            for (int i = 1; i < 20; i++)
            {
                var item = manager.GetConnection();
                stack.Push(item);
                Assert.AreEqual(i, manager.ConnectionPool.Count, string.Format("Iteration {0}", i));
            }

            while (stack.Count > 0)
            {
                var item = stack.Pop();
                int start = manager.ConnectionPool.Count;
                item.Dispose();
                Assert.AreEqual(start - 1, manager.ConnectionPool.Count);
            }

            Assert.AreEqual(0, manager.ConnectionPool.Count);
        }

        [Test]
        public void CleanupCommandsTest()
        {
            var manager = TestSettings.CreateDbManager();
            var stack = new Stack<IDbCommand>();

            Assert.AreEqual(0, manager.ConnectionPool.Count);
            for (int i = 1; i < 20; i++)
            {
                var item = manager.GetCommand();
                stack.Push(item);
                Assert.AreEqual(i, manager.ConnectionPool.Count, string.Format("Iteration {0}", i));
            }

            while (stack.Count > 0)
            {
                var item = stack.Pop();
                int start = manager.ConnectionPool.Count;
                item.Dispose();
                Assert.AreEqual(start - 1, manager.ConnectionPool.Count);
            }

            Assert.AreEqual(0, manager.ConnectionPool.Count);
        }

        [Test]
        public void ConnectionTest()
        {
            var manager = TestSettings.CreateDbManager();

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(0, manager.ConnectionPool.Count, string.Format("Iteration {0}", i));
                IPoolableDbConnection pConn;
                IDbConnection conn;
                using (pConn = manager.GetConnection())
                {
                    Assert.AreEqual(1, manager.ConnectionPool.Count, string.Format("Iteration {0}", i));
                    conn = pConn.Connection;
                    Assert.IsNotNull(conn);
                    Assert.AreEqual(ConnectionState.Open, conn.State);
                }
                Assert.AreEqual(ConnectionState.Closed, conn.State);
            }
        }

        [Test]
        public void CommandTest()
        {
            var manager = TestSettings.CreateDbManager();

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(0, manager.ConnectionPool.Count, string.Format("Iteration {0}", i));
                IDbCommand cmd;
                using (cmd = manager.GetCommand())
                {
                    Assert.AreEqual(1, manager.ConnectionPool.Count, string.Format("Iteration {0}", i));
                    Assert.IsNotNull(cmd);
                    cmd.CommandText = "SELECT 500 + 100";
                    using (var r = cmd.ExecuteReader())
                    {
                        r.Read();
                        Assert.AreEqual("600", r[0].ToString());
                    }
                }
            }
        }

        [Test]
        public void CommandParametersTest()
        {
            var manager = TestSettings.CreateDbManager();

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(0, manager.ConnectionPool.Count, string.Format("Iteration {0}", i));

                IDbCommand cmd;
                using (cmd = manager.GetCommand())
                {
                    Assert.AreEqual(1, manager.ConnectionPool.Count, string.Format("Iteration {0}", i));
                    Assert.IsNotNull(cmd);

                    cmd.CommandText = "SELECT @left + @right";

                    var left = new MySqlParameter("@left", null) { Value = 500 };
                    var right = new MySqlParameter("@right", null) { Value = 100 };
                    cmd.Parameters.Add(left);
                    cmd.Parameters.Add(right);

                    using (var r = cmd.ExecuteReader())
                    {
                        r.Read();
                        Assert.AreEqual("600", r[0].ToString());
                    }

                    Assert.AreNotEqual(0, cmd.Parameters.Count);
                    Assert.IsFalse(string.IsNullOrEmpty(cmd.CommandText));
                }
            }
        }

        [Test]
        public void CommandStringTest()
        {
            var manager = TestSettings.CreateDbManager();

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(0, manager.ConnectionPool.Count, string.Format("Iteration {0}", i));
                using (var cmd = manager.GetCommand("SELECT 500 + 100"))
                {
                    Assert.AreEqual(1, manager.ConnectionPool.Count, string.Format("Iteration {0}", i));
                    Assert.IsNotNull(cmd);
                    Assert.AreEqual("SELECT 500 + 100", cmd.CommandText);
                    using (var r = cmd.ExecuteReader())
                    {
                        r.Read();
                        Assert.AreEqual("600", r[0].ToString());
                    }
                }
            }
        }

        [Test]
        public void ExecuteNonQueryTest()
        {
            var manager = TestSettings.CreateDbManager();

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(0, manager.ConnectionPool.Count, string.Format("Iteration {0}", i));
                int ret = manager.ExecuteNonQuery("SELECT 500 + 100");
                Assert.AreEqual(-1, ret);
            }
        }

        [Test]
        public void ExecuteReaderTest()
        {
            var manager = TestSettings.CreateDbManager();

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(0, manager.ConnectionPool.Count, string.Format("Iteration {0}", i));
                using (var r = manager.ExecuteReader("SELECT 500 + 100"))
                {
                    Assert.AreEqual(1, manager.ConnectionPool.Count, string.Format("Iteration {0}", i));
                    Assert.IsFalse(r.IsClosed);
                    r.Read();
                    Assert.AreEqual("600", r[0].ToString());
                }
            }
        }

        [Test]
        public void ExecuteReader2Test()
        {
            var manager = TestSettings.CreateDbManager();

            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(0, manager.ConnectionPool.Count, string.Format("Iteration {0}", i));
                using (var r = manager.ExecuteReader("SELECT 500 + 100", CommandBehavior.SingleResult))
                {
                    Assert.AreEqual(1, manager.ConnectionPool.Count, string.Format("Iteration {0}", i));
                    Assert.IsFalse(r.IsClosed);
                    r.Read();
                    Assert.AreEqual("600", r[0].ToString());
                }
            }
        }
    }
}
