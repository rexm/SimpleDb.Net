using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cucumber.SimpleDb
{
    /// <summary>
    /// Represents a collection of <c>Cucumber.SimpleDb.ISimpleDbItem</c> instances in a <c>Cucumber.SimpleDb.ISimpleDbDomain</c>.
    /// </summary>
    public interface ISimpleDbItemCollection : IQueryable<ISimpleDbItem>
    {
        /// <summary>
        /// Creates a new item.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="name"/> is null or empty</exception>
        /// <param name="name">The name of the item to create.</param>
        /// <returns>The new item.</returns>
        ISimpleDbItem Add(string name);

        /// <summary>
        /// Creates a new item with the specified initial attributes
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="name"/> is null or empty</exception>
        /// <param name="name">The name of the item to create.</param>
        /// <param name="values">A collection of name-value pairs to initialize the <c>Cucumber.SimpleDb.ISimpleDbItem.Attributes</c> collection.</param>
        /// <returns>The new item.</returns>
        ISimpleDbItem Add(string name, Dictionary<string, SimpleDbAttributeValue> values);

        /// <summary>
        /// Creates a new item with the specified initial attributes and associated commit condition.
        /// <para>A new <c>Cucumber.SimpleDb.ISimpleDbItem</c> instance will be created, but when <c>Cucumber.SimpleDb.ISimpleDbContext.SubmitChanges()</c> is invoked, the item will only be committed to SimpleDb if the condition evaluates true.</para>
        /// </summary>
        /// <see cref="Cucumber.SimpleDb.ISimpleDbContext"/>
        /// <param name="name">The name of the item to create.</param>
        /// <param name="values">A collection of name-value pairs to initialize the <c>Cucumber.SimpleDb.ISimpleDbItem.Attribtues</c> collection.</param>
        /// <param name="conditionAttribute">The name of the conditional attribute to check.</param>
        /// <param name="conditionValue">The expected value of the conditional attribute.</param>
        /// <returns>The new item.</returns>
        ISimpleDbItem AddWhen(string name, Dictionary<string, SimpleDbAttributeValue> values, string conditionAttribute, SimpleDbAttributeValue conditionValue);

        /// <summary>
        /// Creates a new item with the associated commit condition.
        /// <para>A new <c>Cucumber.SimpleDb.ISimpleDbItem</c> instance will be created, but when <c>Cucumber.SimpleDb.ISimpleDbContext.SubmitChanges()</c> is invoked, the item will only be committed to SimpleDb if the condition evaluates true.</para>
        /// </summary>
        /// <see cref="Cucumber.SimpleDb.ISimpleDbContext"/>
        /// <param name="name">The name of the item to create.</param>
        /// <param name="conditionAttribute">The name of the conditional attribute to check.</param>
        /// <param name="conditionValue">The expected value of the conditional attribute.</param>
        /// <returns>The new item.</returns>
        ISimpleDbItem AddWhen(string name, string conditionAttribute, SimpleDbAttributeValue conditionValue);

        /// <summary>
        /// Gets the item with the specified name.
        /// <para>If no item with the specified name exists, null will be returned.</para>
        /// </summary>
        /// <param name="itemName">The name of the item to return.</param>
        /// <returns>The <c>Cucumber.SimpleDb.ISimpleDbItem</c> instance if exists; otherwise null.</returns>
        ISimpleDbItem this[string itemName] { get; }

        /// <summary>
        /// Gets the total number of items in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the parent domain.
        /// </summary>
        ISimpleDbDomain Domain { get; }
    }
}
