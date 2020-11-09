using System.Collections.Generic;
using System.Collections.ObjectModel;
using Miru.Domain;
using NUnit.Framework;
// using Shoppers.Domain;
// using Shoppers.Features.Carts;
using Shouldly;

namespace Miru.Tests
{
    public class TypeExtensionsTest
    {
        // [Test]
        // public void Should_return_if_type_is_request_query()
        // {
        //     new CartIndex.Query().GetType().IsRequestQuery().ShouldBeTrue();
        //     
        //     new CartAdd.Command().GetType().IsRequestQuery().ShouldBeFalse();
        // }
        //
        // [Test]
        // public void Should_return_if_type_is_request_command()
        // {
        //     new CartIndex.Query().GetType().IsRequestCommand().ShouldBeFalse();
        //     
        //     new CartAdd.Command().GetType().IsRequestCommand().ShouldBeTrue();
        // }
        //
        // [Test]
        // public void Check_if_type_implements_another()
        // {
        //     var category = new Category();
        //     
        //     category.Products.GetType().Implements(typeof(IEnumerable<IEntity>)).ShouldBeTrue();
        // }

        [Test]
        public void Return_if_type_is_enumerable_of_something()
        {
            new Continent().Countries.GetType().ImplementsEnumerableOfSomething().ShouldBeTrue();
            
            new Continent().Organizations.GetType().ImplementsEnumerableOfSomething().ShouldBeFalse();
        }
        
        [Test]
        public void Return_if_type_is_enumerable_of_a_type()
        {
            typeof(IEnumerable<Country>).ImplementsEnumerableOf<IEntity>().ShouldBeTrue();
            
            new Continent().Countries.GetType().ImplementsEnumerableOf<Country>().ShouldBeTrue();
            
            new Continent().Countries.GetType().ImplementsEnumerableOf<Continent>().ShouldBeFalse();
        }

        [Test]
        public void Return_if_type_implements_generic_of_something()
        {
            typeof(Collection<Country>).ImplementsGenericOf(typeof(Collection<>)).ShouldBeTrue();
            
            typeof(Collection<Country>).ImplementsGenericOf(typeof(ICollection<>)).ShouldBeTrue();

            typeof(string).ImplementsGenericOf(typeof(IEnumerable<>)).ShouldBeTrue();

            typeof(string).ImplementsGenericOf(typeof(ICollection<>)).ShouldBeFalse();
        }

        public class Continent
        {
            public IEnumerable<Country> Countries { get; set; } = new List<Country>();
            
            public List Organizations { get; set; } = new List();
        }
        
        public class Country : IEntity
        {
            public long Id { get; }
        }
    }
}