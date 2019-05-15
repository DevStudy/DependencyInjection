// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection.Specification.Fakes;
using Microsoft.Extensions.DependencyInjection.Specification.Tests.Fakes;

namespace Microsoft.Extensions.DependencyInjection.Tests.Fakes
{
    /// <summary>
    /// 构造器中依赖一个私有构造器的类
    /// </summary>
    public class ClassDependsOnPrivateConstructorClass
    {
        public ClassDependsOnPrivateConstructorClass(ClassWithPrivateCtor value)
        {

        }
    }
}
