namespace PoetryGlobal.Interfaces
{
    /// <summary>
    /// Represents models that do not necessarily come from the database
    /// and may potentially be persisted if their fields are valid.
    /// Usually, that means that fields the corresponding database table
    /// has a NOT NULL constraint on are not null on the model,
    /// and the model does not have a primary key set.
    /// </summary>
    public interface IModel
    {
        bool CanBePersisted();
    }
}