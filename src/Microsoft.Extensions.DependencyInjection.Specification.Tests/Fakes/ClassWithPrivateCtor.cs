// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection.Specification.Tests.Fakes
{

    /// <summary>
    /// 构造一个没有公有构造器的类
    /// </summary>
    public class ClassWithPrivateCtor
    {
        private ClassWithPrivateCtor()
        {
        }
    }
}