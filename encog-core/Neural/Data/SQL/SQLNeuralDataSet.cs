﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Util.DB;
using Encog.Neural.Data.Basic;

namespace Encog.Neural.Data.SQL
{
    public class SQLNeuralDataSet : INeuralDataSet, IEnumerable<INeuralDataPair>
    {
        public class SQLNeuralEnumerator : IEnumerator<INeuralDataPair>
        {
            private SQLNeuralDataSet owner;


            private INeuralDataPair current;

            /// <summary>
            /// Holds results from the SQL query.
            /// </summary>
            private RepeatableStatement.Results results;


            public SQLNeuralEnumerator(SQLNeuralDataSet owner)
            {
                this.owner = owner;
                this.results = owner.statement.ExecuteQuery();
            }

            public INeuralDataPair Current
            {
                get
                {
                    if (this.current == null)
                    {
                        MoveNext();
                    }
                    return this.current;
                }
            }

            public void Dispose()
            {
                this.results.Close();
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    if (this.current == null)
                    {
                        MoveNext();
                    }
                    return this.current;
                }
            }

            public bool MoveNext()
            {
                if (!this.results.DataReader.NextResult())
                    return false;
                INeuralData input = new BasicNeuralData(owner.inputSize);
                INeuralData ideal = null;

                for (int i = 1; i <= owner.inputSize; i++)
                {
                    input[i - 1] = this.results.DataReader.GetDouble(i);
                }

                if (owner.idealSize > 0)
                {
                    ideal =
                    new BasicNeuralData(owner.idealSize);
                    for (int i = 1; i <= owner.idealSize; i++)
                    {
                        ideal[i - 1] =
                            this.results.DataReader.GetDouble(i + owner.inputSize);
                    }

                }

                this.current = new BasicNeuralDataPair(input, ideal);
                return true;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }


        }

        /// <summary>
        /// What is the size of the input data?
        /// </summary>
        private int inputSize;

        /// <summary>
        /// What is the size of the ideal data?
        /// </summary>
        private int idealSize;

        /// <summary>
        /// The database connection.
        /// </summary>
        private RepeatableConnection connection;

        /// <summary>
        /// The SQL statement being used.
        /// </summary>
        private RepeatableStatement statement;

        /// <summary>
        /// Create a SQL neural data set.
        /// </summary>
        /// <param name="sql">The SQL to execute.</param>
        /// <param name="inputSize">The size of the input data being read.</param>
        /// <param name="idealSize">The size of the ideal output data being read.</param>
        /// <param name="connectString">The connection string.</param>
        public SQLNeuralDataSet(String sql, int inputSize,
                 int idealSize, String connectString)
        {
            this.inputSize = inputSize;
            this.idealSize = idealSize;
            this.connection = new RepeatableConnection(connectString);
            this.statement = this.connection.CreateStatement(sql);
            this.connection.Open();
        }



        public int IdealSize
        {
            get
            {
                return this.idealSize;
            }
        }

        public int InputSize
        {
            get
            {
                return this.inputSize;
            }
        }

        public void Add(INeuralData data1)
        {
            throw new NotImplementedException();
        }

        public void Add(INeuralData inputData, INeuralData idealData)
        {
            throw new NotImplementedException();
        }

        public void Add(INeuralDataPair inputData)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
        }

        public IEnumerator<INeuralDataPair> GetEnumerator()
        {
            return new SQLNeuralEnumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new SQLNeuralEnumerator(this);
        }

    }
}
