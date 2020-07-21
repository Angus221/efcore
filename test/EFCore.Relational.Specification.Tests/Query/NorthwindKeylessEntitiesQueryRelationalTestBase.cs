// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Query
{
    public abstract class NorthwindKeylessEntitiesQueryRelationalTestBase<TFixture> : NorthwindKeylessEntitiesQueryTestBase<TFixture>
        where TFixture : NorthwindQueryFixtureBase<NoopModelCustomizer>, new()
    {
        protected NorthwindKeylessEntitiesQueryRelationalTestBase(TFixture fixture)
            : base(fixture)
        {
        }

        protected virtual bool CanExecuteQueryString => false;

        [ConditionalFact]
        public virtual void Projecting_collection_correlated_with_keyless_entity_throws()
        {
            using var context = CreateContext();
            var message = Assert.Throws<NotSupportedException>(() => context.Set<ProductQuery>().Select(pv => new
            {
                pv.ProductID,
                pv.ProductName,
                OrderDetailIds = context.Set<OrderDetail>().Where(od => od.ProductID == pv.ProductID).ToList()
            }).OrderBy(x => x.ProductID).Take(2).ToList()).Message;

            Assert.Equal(RelationalStrings.ProjectingCollectionOnKeylessEntityNotSupported, message);
        }

        protected override QueryAsserter CreateQueryAsserter(TFixture fixture)
            => new RelationalQueryAsserter(fixture, RewriteExpectedQueryExpression, RewriteServerQueryExpression, canExecuteQueryString: CanExecuteQueryString);
    }
}
