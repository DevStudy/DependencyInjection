// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Options for configuring various behaviors of the default <see cref="IServiceProvider"/> implementation.
    /// </summary>
    public class ServiceProviderOptions
    {
        //NOTE：它有两次调用，可以认为是个Const定义。这儿的目的应该只是为了一次初始化，多次使用。节省一点点空间和时间。
        //NOTE: 可能 是别的地方有另外继承基类， 反过来再修改添加行为的时候 加出来的， 或许可能再加一个类来实现更好
        // Avoid allocating objects in the default case
        internal static readonly ServiceProviderOptions Default = new ServiceProviderOptions();

        /// <summary>
        /// <c>true</c> to perform check verifying that scoped services never gets resolved from root provider; otherwise <c>false</c>.
        /// </summary>
        public bool ValidateScopes { get; set; }
    }
}
