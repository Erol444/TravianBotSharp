using System;
using TbsCore.Models.MapModels;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TbsCore.Models.SendTroopsModels
{
    /// <summary>
    /// Used when parsing rally point movements (overview tab)
    /// </summary>
    public class TroopsMovementRallyPoint : TroopsMovementBase, IEquatable<TroopsMovementRallyPoint>
    {
        /// <summary>
        /// Type of movement
        /// </summary>
        public MovementTypeRallyPoint MovementType { get; set; }

        /// <summary>
        /// Tribe of the troops
        /// </summary>
        public TribeEnum Tribe { get; set; }

        /// <summary>
        /// Source coordinates, from where the troops are coming from or came (reinforcements)
        /// </summary>
        public Coordinates SourceCoordinates { get; set; }

        public bool Equals(TroopsMovementRallyPoint other)
        {
            if (other == null || this.Troops.Length != other.Troops.Length) return false;

            for (int i = 0; i < this.Troops.Length; i++)
            {
                if (this.Troops[i] != other.Troops[i]) return false;
            }

            if (this.Arrival.CompareTo(other.Arrival) != 0) return false;
            if (!this.SourceCoordinates.Equals(other.SourceCoordinates)) return false;
            if (!this.TargetCoordinates.Equals(other.TargetCoordinates)) return false;
            if (this.MovementType != other.MovementType) return false;
            return true;
        }
    }
}