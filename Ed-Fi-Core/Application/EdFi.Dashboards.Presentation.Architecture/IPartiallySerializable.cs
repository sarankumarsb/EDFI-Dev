// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

namespace EdFi.Dashboards.Presentation.Architecture
{
    /// <summary>
    /// Supports the ability to serialize only part of a model being returned.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <remarks>This concept of partially serializable model is really covering for the fact that there are certain
    /// cases whether a custom controller is augmenting the model that is returned from the service layer.
    /// It would probably be better to eliminate this concept, and separate the controller and the service into
    /// two separately addressable resources, rather than strip off part of the model using this IPartiallySerializable
    /// interface.
    /// </remarks>
    public interface IPartiallySerializable<out TModel>
    {
        /// <summary>
        /// Gets the serializable portion of the model.
        /// </summary>
        TModel SerializableModel { get; }
    }
}