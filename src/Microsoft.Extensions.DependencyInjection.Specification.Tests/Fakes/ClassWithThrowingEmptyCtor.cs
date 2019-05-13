// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.DependencyInjection.Specification.Tests.Fakes
{
    /// <summary>
    /// 无参构造直抛异常
    /// </summary>
    public class ClassWithThrowingEmptyCtor
    {
        public ClassWithThrowingEmptyCtor()
        {
            throw new Exception(nameof(ClassWithThrowingEmptyCtor));
        }
    }
}