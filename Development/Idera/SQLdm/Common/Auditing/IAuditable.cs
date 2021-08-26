// -----------------------------------------------------------------------
// <copyright file="IAuditable.cs" company="Idera">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Idera.SQLdm.Common.Auditing
{
    /// <summary>
    /// Interface for all Auditable objects
    /// </summary>
    public interface IAuditable
    {
        /// <summary>
        /// Returns an Auditable Entity
        /// </summary>
        /// <returns></returns>
        AuditableEntity GetAuditableEntity();

        /// <summary>
        /// Returns an Auditable Entity based on an oldValue
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        AuditableEntity GetAuditableEntity(IAuditable oldValue);
    }
}
