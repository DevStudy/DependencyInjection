using Microsoft.Extensions.DependencyInjection.Specification.Fakes;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Extensions.DependencyInjection.Tests
{
    public class StudyDemo1
    {



        [Fact]
        public void TestFor_ISet()
        {
            var x1 = new FakeService();
            var x2 = new FakeService();

            var ds1 = new HashSet<FakeService> { x1, x2 };
            var ds2 = new HashSet<FakeService> { x2, x1 };
            var ds3 = new HashSet<FakeService> { x2, x1, x2 };
            Assert.True(ds1.SetEquals(ds2));
            Assert.True(ds1.SetEquals(ds3));
            //NOTE:只要集合中的元素一就是那几个就行了。


        }



        [Fact]
        public void DifferenceBetweenIsGenericTypeAndIsGenericTypeDefinition()
        {
            var listInt = typeof(List<int>);
            var typeDef = listInt.GetGenericTypeDefinition(); // gives typeof(List<>)

            var listDef = typeof(List<>);
            var listStr = listDef.MakeGenericType(typeof(string)); //gives typeof(List<String>)


            //Console.WriteLine(l);
            Assert.Null(listStr.DeclaringType);
            //Assert.IsType<List<String>>(listStr.UnderlyingSystemType);
        }




    }
}