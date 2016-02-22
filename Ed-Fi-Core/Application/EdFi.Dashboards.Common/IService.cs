// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;

namespace EdFi.Dashboards.Common
{
    public interface IService<in TRequest, out TResponse>
    {
        TResponse Get(TRequest request);
    }

    #region For Future Use (Commented out)

    ///// <summary>
    ///// Handles an HTTP GET request.
    ///// </summary>
    ///// <typeparam name="TRequest">The <see cref="Type"/> of the request containing the identity of the resource to be retrieved.</typeparam>
    ///// <typeparam name="TResponse">The <see cref="Type"/> of the response model.</typeparam>
    //public interface IGetHandler<in TRequest, out TResponse>
    //{
    //    /// <summary>
    //    /// Gets a collection or item-level resource, as identified by the request.
    //    /// </summary>
    //    /// <param name="request">The request containing the identity of the collection or item-level resource to be retrieved.</param>
    //    /// <returns>A model of type <see cref="TResponse"/> containing a representation of the resource, complete with all of its links.</returns>
    //    /// <exception cref="ApplicationException">Exception containing a user-viewable message.</exception>
    //    /// <exception cref="Exception">Exception containing a system error message, not appropriate for display to the user.</exception>
    //    TResponse Get(TRequest request);
    //}

    #endregion

    /// <summary>
    /// Handles an HTTP POST request.
    /// </summary>
    /// <typeparam name="TRequest">The <see cref="Type"/> of the request containing both the identity and the content of the resource to be created or processed by the handler.</typeparam>
    /// <typeparam name="TResponse">The <see cref="Type"/> of the response model.</typeparam>
    /// <exception cref="ApplicationException">Exception containing a user-viewable message.</exception>
    /// <exception cref="Exception">Exception containing a system error message, not appropriate for display to the user.</exception>
    public interface IPostHandler<in TRequest, out TResponse>
    {
        /// <summary>
        /// Processes a request representing a subordinate resource, generally by adding a new item-level resource.
        /// </summary>
        /// <param name="request">The request containing the identity of a collection-based resource, and the content of the item-level resource to be created or processed.</param>
        /// <returns>A model of type <see cref="TResponse"/> containing a representation of the resource, complete with all of its links.</returns>
        /// <exception cref="ApplicationException">Exception containing a user-viewable message.</exception>
        /// <exception cref="Exception">Exception containing a system error message, not appropriate for display to the user.</exception>
        TResponse Post(TRequest request);   
    }

    /// <summary>
    /// Handles an HTTP PUT request.
    /// </summary>
    /// <typeparam name="TRequest">The <see cref="Type"/> of the request containing both the identity and the content of the resource to be updated or created.</typeparam>
    /// <typeparam name="TResponse">The <see cref="Type"/> of the response model containing the resource representation with the appropriate resource links.</typeparam>
    public interface IPutHandler<in TRequest, out TResponse>
    {
        /// <summary>
        /// Creates or updates the resource specified by the request.
        /// </summary>
        /// <param name="request">The request containing both the identity and the content of the resource to be updated or created.</param>
        /// <param name="created">true if the resource was created; false if an existing resource was updated.</param>
        /// <returns>A model of type <see cref="TResponse"/> containing a representation of the entity, complete with all of its links.</returns>
        /// <exception cref="ApplicationException">Exception containing a user-viewable message.</exception>
        /// <exception cref="Exception">Exception containing a system error message, not appropriate for display to the user.</exception>
        TResponse Put(TRequest request, out bool created);
    }

    /// <summary>
    /// Handles an HTTP DELETE request.
    /// </summary>
    /// <typeparam name="TRequest">The request identifying the resource to be deleted.</typeparam>
    public interface IDeleteHandler<in TRequest>
    {
        /// <summary>
        /// Deletes the specified resource.
        /// </summary>
        /// <param name="request">The request containing the identity of the collection or item-level resource to be deleted.</param>
        void Delete(TRequest request);
    }

    public enum PostAction
    {
        /// <summary>
        /// Indicates the item or items should be added.
        /// </summary>
        Add,

        /// <summary>
        /// Indicates the item or items should be removed.
        /// </summary>
        Remove,

        /// <summary>
        /// Indicates the item or items should replace an existing corresponding item.
        /// </summary>
        Set,

        /// <summary>
        /// Indicates the item should be deleted.
        /// </summary>
        Delete
    }
}
