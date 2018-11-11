using System;

namespace LevelEditor.Models
{
    public abstract class FileKey : IEquatable<FileKey> {

        public int Id { get; set; }
        public virtual string ContentPath { get; set; }
        public bool Equals(FileKey other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FileKey)obj);
        }

        public override int GetHashCode() {
            return Id;
        }

        public override string ToString() {
            return $"[{Id}] {ContentPath ?? "NULL"}";
        }
    }
}