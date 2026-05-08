using System;

namespace SprintRoom.Domain.Entities
{
    /// <summary>
    /// Base class for all domain entities in the SprintRoom application.
    /// This abstract class ensures that every entity has a unique identifier and consistent audit fields.
    /// By centralizing these properties, we maintain consistency across the domain model
    /// and avoid code duplication for basic entity attributes.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// Using Guid provides a globally unique identifier that can be generated independently of the database.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was first created.
        /// Useful for audit trails and chronological sorting.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was last updated.
        /// Allows tracking of the most recent modifications for synchronization or audit purposes.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// Protected constructor to allow initialization of the Id and CreatedAt fields
        /// upon the creation of derived entities.
        /// </summary>
        protected BaseEntity()
        {
            // Initializes a new unique identifier for the entity.
            Id = Guid.NewGuid();
            
            // Sets the initial creation time to the current time in UTC to avoid timezone issues.
            CreatedAt = DateTimeOffset.UtcNow;
            
            // At creation, the updated time is the same as the creation time.
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
