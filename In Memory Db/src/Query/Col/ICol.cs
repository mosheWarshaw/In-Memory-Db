﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InMemoryDb.Table;

namespace InMemoryDb
{
    public interface ICol
    {
        string SourceColumnName { get; }
        string ResultColumnName { get; }

        void SetSourceColumnWrapper(IColumnWrapper columnWrapper);
        void SetSameRowAccessor(SameRowAccessor sameRowAccessor);

        int? GetPkIndex(int fkIndex);

        void TemporarilyAdd(int index);
        void PermanentlyAdd();

        IColumn GetColumn();

        void AddNull();

        Type GetResultType();
    }
}
