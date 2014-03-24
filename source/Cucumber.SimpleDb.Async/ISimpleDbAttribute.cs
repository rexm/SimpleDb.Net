namespace Cucumber.SimpleDb.Async
{
    /// <summary>
    /// Represents a single attribute on a <c>Cucumber.SimpleDb.ISimpleDbItem</c> instance
    /// </summary>
    public interface ISimpleDbAttribute
    {
        /// <summary>
        /// Gets and sets the value of the attribute
        /// </summary>
        SimpleDbAttributeValue Value { get; set; }

        /// <summary>
        /// Gets the name of the attribute
        /// </summary>
        string Name { get; }
    }
}