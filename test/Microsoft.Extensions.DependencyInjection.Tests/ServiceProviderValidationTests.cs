// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;

namespace Microsoft.Extensions.DependencyInjection.Tests
{
    /// <summary>
    /// NOTE：Scoped是一种特殊的范围属性，它不能被GetService直接取得。？？
    /// </summary>
    public class ServiceProviderValidationTests
    {

        //TODO:这个不能明白了。 AddSingleton就能获得。AddScoped却不能获得？
        [Fact]
        public void GetService_Throws_WhenGetServiceForScopedServiceIsCalledOnRoot()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IBar, Bar>();
            var serviceProvider = serviceCollection.BuildServiceProvider(validateScopes: true);

            // Act + Assert
            var exception = Assert.Throws<InvalidOperationException>(() => serviceProvider.GetService(typeof(IBar)));
            Assert.Equal($"Cannot resolve scoped service '{typeof(IBar)}' from root provider.", exception.Message);
        }

       
        [Fact]
        public void GetService_Ok_WhenGetServiceForTransientServiceIsCalledOnRoot()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IBaz, Baz>();
            var serviceProvider = serviceCollection.BuildServiceProvider(validateScopes: true);

            // Act + Assert
            var result = (IBaz)serviceProvider.GetService(typeof(IBaz));
            Assert.NotNull(result);
            result.Value = 5;

            var result2 = (IBaz)serviceProvider.GetService(typeof(IBaz));
            Assert.Equal(0,result2.Value);
        }


        [Fact]
        public void GetService_Ok_WhenScopedIsInjectedIntoSingleton()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IFoo, Foo>(); //NOTE： Foo里有IBar，
            serviceCollection.AddSingleton<IBar, Bar2>(); //NOTE：Bar2里有IBaz。
            serviceCollection.AddSingleton<IBaz, Baz>();
            var serviceProvider = serviceCollection.BuildServiceProvider(validateScopes: true);


            // Act + Assert
            var result = (IFoo)serviceProvider.GetService(typeof(IFoo));
            Assert.NotNull(result);
            result.Bar.Baz.Value = 5;


            var result2 = (IFoo)serviceProvider.GetService(typeof(IFoo));
            Assert.Equal(5, result2.Bar.Baz.Value);
        }


        //NOTE：这儿依次有引用依赖，所以必须都注册为Singleton才行了。
        [Fact]
        public void GetService_Throws_WhenScopedIsInjectedIntoSingletonThroughTransient()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IFoo, Foo>(); //NOTE： Foo里有IBar，
            serviceCollection.AddTransient<IBar, Bar2>(); //NOTE：Bar2里有IBaz。
            serviceCollection.AddScoped<IBaz, Baz>();
            var serviceProvider = serviceCollection.BuildServiceProvider(validateScopes: true);

            // Act + Assert
            var exception = Assert.Throws<InvalidOperationException>(() => serviceProvider.GetService(typeof(IFoo)));
            Assert.Equal($"Cannot consume scoped service '{typeof(IBaz)}' from singleton '{typeof(IFoo)}'.", exception.Message);
        }

        //NOTE：当单例中有服务是以Scoped来注册时会抛异常。
        [Fact]
        public void GetService_Throws_WhenScopedIsInjectedIntoSingletonThroughSingleton()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IFoo, Foo>();
            serviceCollection.AddSingleton<IBar, Bar2>(); //NOTE:这里有IBaz的引用
            serviceCollection.AddScoped<IBaz, Baz>();
            var serviceProvider = serviceCollection.BuildServiceProvider(validateScopes: true);

            // Act + Assert
            var exception = Assert.Throws<InvalidOperationException>(() => serviceProvider.GetService(typeof(IFoo)));
            Assert.Equal($"Cannot consume scoped service '{typeof(IBaz)}' from singleton '{typeof(IBar)}'.", exception.Message);
        }


        //NOTE：不一样的Scope注册无法提供统一的服务
        [Fact]
        public void GetService_Throws_WhenGetServiceForScopedServiceIsCalledOnRootViaTransient()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IFoo, Foo>();
            serviceCollection.AddScoped<IBar, Bar>();
            //serviceCollection.AddTransient<IBar, Bar>(); //NOTE：增加一个同样Scope的就不会抛出异常了。
            var serviceProvider = serviceCollection.BuildServiceProvider(validateScopes: true);

            // Act + Assert
            var exception = Assert.Throws<InvalidOperationException>(() => serviceProvider.GetService(typeof(IFoo)));
            Assert.Equal($"Cannot resolve '{typeof(IFoo)}' from root provider because it requires scoped service '{typeof(IBar)}'.", exception.Message);
        }

        //NOTE：当使用IServiceScopeFactory来作为构造参数时，可以取得Singleton服务实例。
        [Fact]
        public void GetService_DoesNotThrow_WhenScopeFactoryIsInjectedIntoSingleton()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IBoo, Boo>();
            var serviceProvider = serviceCollection.BuildServiceProvider(true);

            // Act + Assert
            var result = serviceProvider.GetService(typeof(IBoo));
            Assert.NotNull(result);
        }

        //NOTE:Foo的构造中引用了IBar，如果它们的注册范围统一为Singleton，那么就能成为Singleton来提供服务。
        [Fact]
        public void GetService_WhenScopedIsInjectedIntoSingleton()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IFoo, Foo>();
            serviceCollection.AddSingleton<IBar, Bar>();
            var serviceProvider = serviceCollection.BuildServiceProvider(validateScopes: true);

            // Act + Assert
            var result = serviceProvider.GetService(typeof(IFoo));
            Assert.NotNull(result);
        }
        //NOTE:Foo的构造中引用了IBar，如果它们的注册范围不统一，那么无法成为Singleton来提供服务。
        [Fact]
        public void GetService_Throws_WhenScopedIsInjectedIntoSingleton()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IFoo, Foo>();
            serviceCollection.AddScoped<IBar, Bar>();
            var serviceProvider = serviceCollection.BuildServiceProvider(validateScopes: true);

            // Act + Assert
            var exception = Assert.Throws<InvalidOperationException>(() => serviceProvider.GetService(typeof(IFoo)));
            Assert.Equal($"Cannot consume scoped service '{typeof(IBar)}' from singleton '{typeof(IFoo)}'.", exception.Message);
        }

        public interface IFoo
        {
            IBar Bar { get; }
        }

        private class Foo : IFoo
        {
            public Foo(IBar bar)
            {
                Bar = bar;
            }

            public IBar Bar { get; }

        }

        public interface IBar
        {
            IBaz Baz { get; }
        }

        private class Bar : IBar
        {
            public IBaz Baz { get; }
        }

        private class Bar2 : IBar
        {
            public Bar2(IBaz baz)
            {
                Baz = baz;
            }

            public IBaz Baz { get; }
        }

        public interface IBaz
        {
            int Value { get; set; }
        }

        private class Baz : IBaz
        {
            public int Value { get; set; }
        }

        public interface IBoo
        {
        }

        private class Boo : IBoo
        {
            public Boo(IServiceScopeFactory scopeFactory)
            {
            }
        }
    }
}
