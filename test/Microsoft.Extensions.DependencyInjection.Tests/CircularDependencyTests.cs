// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.DependencyInjection.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection.Tests.Fakes.CircularReferences;
using Xunit;

namespace Microsoft.Extensions.DependencyInjection.Tests
{
    /// <summary>
    /// 关于循环引用的测试
    /// </summary>
    public class CircularDependencyTests
    {
        /// <summary>
        /// NOTE：类构造时传参自身不能取得实例。
        /// </summary>
        [Fact]
        public void SelfCircularDependency()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<SelfCircularDependency>()
                .BuildServiceProvider();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                serviceProvider.GetRequiredService<SelfCircularDependency>());

            Assert.Equal(
                Resources.FormatCircularDependencyException(typeof(SelfCircularDependency)),
                exception.Message);
        }

        /// <summary>
        /// NOTE：自已引用自已的泛型服务也是无法取得服务实例的。
        /// </summary>
        [Fact]
        public void SelfCircularDependencyGenericDirect()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<SelfCircularDependencyGeneric<string>>()
                .BuildServiceProvider();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                serviceProvider.GetRequiredService<SelfCircularDependencyGeneric<string>>());

            Assert.Equal(
                Resources.FormatCircularDependencyException(typeof(SelfCircularDependencyGeneric<string>)),
                exception.Message);
        }

        [Fact]
        public void SelfCircularDependencyGenericIndirect()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<SelfCircularDependencyGeneric<int>>()
                .AddTransient<SelfCircularDependencyGeneric<string>>()
                .BuildServiceProvider();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                serviceProvider.GetRequiredService<SelfCircularDependencyGeneric<int>>());

            Assert.Equal(
                Resources.FormatCircularDependencyException(typeof(SelfCircularDependencyGeneric<string>)),
                exception.Message);
        }

        [Fact]
        public void NoCircularDependencyGeneric()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton(new SelfCircularDependencyGeneric<string>()) //NOTE：已有一个单例后，即可使用非循环引用来取得服务实例了。
                .AddTransient<SelfCircularDependencyGeneric<int>>()
                .BuildServiceProvider();

            // This will not throw because we are creating an instace of the first time
            // using the parameterless constructor which has no circular dependency
            var resolvedService = serviceProvider.GetRequiredService<SelfCircularDependencyGeneric<int>>();
            Assert.NotNull(resolvedService);
        }

        [Fact]
        public void SelfCircularDependencyWithInterface()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<ISelfCircularDependencyWithInterface, SelfCircularDependencyWithInterface>()
                .AddTransient<SelfCircularDependencyWithInterface>()
                .BuildServiceProvider();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                serviceProvider.GetRequiredService<SelfCircularDependencyWithInterface>());

            Assert.Equal(
                Resources.FormatCircularDependencyException(typeof(ISelfCircularDependencyWithInterface)),
                exception.Message);
        }

        /// <summary>
        /// NOTE：相互的循环引用也是不行的。无法创建服务实例。
        /// </summary>
        [Fact]
        public void DirectCircularDependency()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<DirectCircularDependencyA>()
                .AddSingleton<DirectCircularDependencyB>()
                .BuildServiceProvider();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                serviceProvider.GetRequiredService<DirectCircularDependencyA>());

            Assert.Equal(
                Resources.FormatCircularDependencyException(typeof(DirectCircularDependencyA)),
                exception.Message);
        }

        [Fact]
        public void IndirectCircularDependency()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IndirectCircularDependencyA>() //NOTE：这个单独是无法创建的。因为它的参数无法自动得取。
                .AddTransient<IndirectCircularDependencyB>()
                .AddTransient<IndirectCircularDependencyC>()
                .BuildServiceProvider();

            var exception = Assert.Throws<InvalidOperationException>(() =>
                serviceProvider.GetRequiredService<IndirectCircularDependencyA>());

            Assert.Equal(
                Resources.FormatCircularDependencyException(typeof(IndirectCircularDependencyA)),
                exception.Message);
        }

        [Fact]
        public void NoCircularDependencySameTypeMultipleTimes()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<NoCircularDependencySameTypeMultipleTimesA>()
                .AddTransient<NoCircularDependencySameTypeMultipleTimesB>()
                .AddTransient<NoCircularDependencySameTypeMultipleTimesC>()
                .BuildServiceProvider();

            var resolvedService = serviceProvider.GetRequiredService<NoCircularDependencySameTypeMultipleTimesA>(); //A的依赖链上没有循环依赖。
            Assert.NotNull(resolvedService);
        }
    }
}