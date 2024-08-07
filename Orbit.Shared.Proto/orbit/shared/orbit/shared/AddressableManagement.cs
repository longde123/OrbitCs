// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: orbit/shared/addressable_management.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Orbit.Shared.Proto {

  /// <summary>Holder for reflection information generated from orbit/shared/addressable_management.proto</summary>
  public static partial class AddressableManagementReflection {

    #region Descriptor
    /// <summary>File descriptor for orbit/shared/addressable_management.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static AddressableManagementReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CilvcmJpdC9zaGFyZWQvYWRkcmVzc2FibGVfbWFuYWdlbWVudC5wcm90bxIM",
            "T3JiaXQuU2hhcmVkGh5vcmJpdC9zaGFyZWQvYWRkcmVzc2FibGUucHJvdG8i",
            "XwohUmVuZXdBZGRyZXNzYWJsZUxlYXNlUmVxdWVzdFByb3RvEjoKCXJlZmVy",
            "ZW5jZRgBIAEoCzInLk9yYml0LlNoYXJlZC5BZGRyZXNzYWJsZVJlZmVyZW5j",
            "ZVByb3RvItkBCiJSZW5ld0FkZHJlc3NhYmxlTGVhc2VSZXNwb25zZVByb3Rv",
            "EkcKBnN0YXR1cxgBIAEoDjI3Lk9yYml0LlNoYXJlZC5SZW5ld0FkZHJlc3Nh",
            "YmxlTGVhc2VSZXNwb25zZVByb3RvLlN0YXR1cxIyCgVsZWFzZRgCIAEoCzIj",
            "Lk9yYml0LlNoYXJlZC5BZGRyZXNzYWJsZUxlYXNlUHJvdG8SGQoRZXJyb3Jf",
            "ZGVzY3JpcHRpb24YAyABKAkiGwoGU3RhdHVzEgYKAk9LEAASCQoFRVJST1IQ",
            "ASJhCiNBYmFuZG9uQWRkcmVzc2FibGVMZWFzZVJlcXVlc3RQcm90bxI6Cgly",
            "ZWZlcmVuY2UYASABKAsyJy5PcmJpdC5TaGFyZWQuQWRkcmVzc2FibGVSZWZl",
            "cmVuY2VQcm90byI5CiRBYmFuZG9uQWRkcmVzc2FibGVMZWFzZVJlc3BvbnNl",
            "UHJvdG8SEQoJYWJhbmRvbmVkGAEgASgIMv8BChVBZGRyZXNzYWJsZU1hbmFn",
            "ZW1lbnQSbwoKUmVuZXdMZWFzZRIvLk9yYml0LlNoYXJlZC5SZW5ld0FkZHJl",
            "c3NhYmxlTGVhc2VSZXF1ZXN0UHJvdG8aMC5PcmJpdC5TaGFyZWQuUmVuZXdB",
            "ZGRyZXNzYWJsZUxlYXNlUmVzcG9uc2VQcm90bxJ1CgxBYmFuZG9uTGVhc2US",
            "MS5PcmJpdC5TaGFyZWQuQWJhbmRvbkFkZHJlc3NhYmxlTGVhc2VSZXF1ZXN0",
            "UHJvdG8aMi5PcmJpdC5TaGFyZWQuQWJhbmRvbkFkZHJlc3NhYmxlTGVhc2VS",
            "ZXNwb25zZVByb3RvQhWqAhJPcmJpdC5TaGFyZWQuUHJvdG9iBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Orbit.Shared.Proto.AddressableReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Orbit.Shared.Proto.RenewAddressableLeaseRequestProto), global::Orbit.Shared.Proto.RenewAddressableLeaseRequestProto.Parser, new[]{ "Reference" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Orbit.Shared.Proto.RenewAddressableLeaseResponseProto), global::Orbit.Shared.Proto.RenewAddressableLeaseResponseProto.Parser, new[]{ "Status", "Lease", "ErrorDescription" }, null, new[]{ typeof(global::Orbit.Shared.Proto.RenewAddressableLeaseResponseProto.Types.Status) }, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Orbit.Shared.Proto.AbandonAddressableLeaseRequestProto), global::Orbit.Shared.Proto.AbandonAddressableLeaseRequestProto.Parser, new[]{ "Reference" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Orbit.Shared.Proto.AbandonAddressableLeaseResponseProto), global::Orbit.Shared.Proto.AbandonAddressableLeaseResponseProto.Parser, new[]{ "Abandoned" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class RenewAddressableLeaseRequestProto : pb::IMessage<RenewAddressableLeaseRequestProto>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<RenewAddressableLeaseRequestProto> _parser = new pb::MessageParser<RenewAddressableLeaseRequestProto>(() => new RenewAddressableLeaseRequestProto());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RenewAddressableLeaseRequestProto> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Orbit.Shared.Proto.AddressableManagementReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RenewAddressableLeaseRequestProto() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RenewAddressableLeaseRequestProto(RenewAddressableLeaseRequestProto other) : this() {
      reference_ = other.reference_ != null ? other.reference_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RenewAddressableLeaseRequestProto Clone() {
      return new RenewAddressableLeaseRequestProto(this);
    }

    /// <summary>Field number for the "reference" field.</summary>
    public const int ReferenceFieldNumber = 1;
    private global::Orbit.Shared.Proto.AddressableReferenceProto reference_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Orbit.Shared.Proto.AddressableReferenceProto Reference {
      get { return reference_; }
      set {
        reference_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as RenewAddressableLeaseRequestProto);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(RenewAddressableLeaseRequestProto other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Reference, other.Reference)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (reference_ != null) hash ^= Reference.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (reference_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Reference);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (reference_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Reference);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (reference_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Reference);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(RenewAddressableLeaseRequestProto other) {
      if (other == null) {
        return;
      }
      if (other.reference_ != null) {
        if (reference_ == null) {
          Reference = new global::Orbit.Shared.Proto.AddressableReferenceProto();
        }
        Reference.MergeFrom(other.Reference);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (reference_ == null) {
              Reference = new global::Orbit.Shared.Proto.AddressableReferenceProto();
            }
            input.ReadMessage(Reference);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            if (reference_ == null) {
              Reference = new global::Orbit.Shared.Proto.AddressableReferenceProto();
            }
            input.ReadMessage(Reference);
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class RenewAddressableLeaseResponseProto : pb::IMessage<RenewAddressableLeaseResponseProto>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<RenewAddressableLeaseResponseProto> _parser = new pb::MessageParser<RenewAddressableLeaseResponseProto>(() => new RenewAddressableLeaseResponseProto());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<RenewAddressableLeaseResponseProto> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Orbit.Shared.Proto.AddressableManagementReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RenewAddressableLeaseResponseProto() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RenewAddressableLeaseResponseProto(RenewAddressableLeaseResponseProto other) : this() {
      status_ = other.status_;
      lease_ = other.lease_ != null ? other.lease_.Clone() : null;
      errorDescription_ = other.errorDescription_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public RenewAddressableLeaseResponseProto Clone() {
      return new RenewAddressableLeaseResponseProto(this);
    }

    /// <summary>Field number for the "status" field.</summary>
    public const int StatusFieldNumber = 1;
    private global::Orbit.Shared.Proto.RenewAddressableLeaseResponseProto.Types.Status status_ = global::Orbit.Shared.Proto.RenewAddressableLeaseResponseProto.Types.Status.Ok;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Orbit.Shared.Proto.RenewAddressableLeaseResponseProto.Types.Status Status {
      get { return status_; }
      set {
        status_ = value;
      }
    }

    /// <summary>Field number for the "lease" field.</summary>
    public const int LeaseFieldNumber = 2;
    private global::Orbit.Shared.Proto.AddressableLeaseProto lease_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Orbit.Shared.Proto.AddressableLeaseProto Lease {
      get { return lease_; }
      set {
        lease_ = value;
      }
    }

    /// <summary>Field number for the "error_description" field.</summary>
    public const int ErrorDescriptionFieldNumber = 3;
    private string errorDescription_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string ErrorDescription {
      get { return errorDescription_; }
      set {
        errorDescription_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as RenewAddressableLeaseResponseProto);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(RenewAddressableLeaseResponseProto other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Status != other.Status) return false;
      if (!object.Equals(Lease, other.Lease)) return false;
      if (ErrorDescription != other.ErrorDescription) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Status != global::Orbit.Shared.Proto.RenewAddressableLeaseResponseProto.Types.Status.Ok) hash ^= Status.GetHashCode();
      if (lease_ != null) hash ^= Lease.GetHashCode();
      if (ErrorDescription.Length != 0) hash ^= ErrorDescription.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (Status != global::Orbit.Shared.Proto.RenewAddressableLeaseResponseProto.Types.Status.Ok) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Status);
      }
      if (lease_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Lease);
      }
      if (ErrorDescription.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(ErrorDescription);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (Status != global::Orbit.Shared.Proto.RenewAddressableLeaseResponseProto.Types.Status.Ok) {
        output.WriteRawTag(8);
        output.WriteEnum((int) Status);
      }
      if (lease_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Lease);
      }
      if (ErrorDescription.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(ErrorDescription);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Status != global::Orbit.Shared.Proto.RenewAddressableLeaseResponseProto.Types.Status.Ok) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Status);
      }
      if (lease_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Lease);
      }
      if (ErrorDescription.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ErrorDescription);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(RenewAddressableLeaseResponseProto other) {
      if (other == null) {
        return;
      }
      if (other.Status != global::Orbit.Shared.Proto.RenewAddressableLeaseResponseProto.Types.Status.Ok) {
        Status = other.Status;
      }
      if (other.lease_ != null) {
        if (lease_ == null) {
          Lease = new global::Orbit.Shared.Proto.AddressableLeaseProto();
        }
        Lease.MergeFrom(other.Lease);
      }
      if (other.ErrorDescription.Length != 0) {
        ErrorDescription = other.ErrorDescription;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Status = (global::Orbit.Shared.Proto.RenewAddressableLeaseResponseProto.Types.Status) input.ReadEnum();
            break;
          }
          case 18: {
            if (lease_ == null) {
              Lease = new global::Orbit.Shared.Proto.AddressableLeaseProto();
            }
            input.ReadMessage(Lease);
            break;
          }
          case 26: {
            ErrorDescription = input.ReadString();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            Status = (global::Orbit.Shared.Proto.RenewAddressableLeaseResponseProto.Types.Status) input.ReadEnum();
            break;
          }
          case 18: {
            if (lease_ == null) {
              Lease = new global::Orbit.Shared.Proto.AddressableLeaseProto();
            }
            input.ReadMessage(Lease);
            break;
          }
          case 26: {
            ErrorDescription = input.ReadString();
            break;
          }
        }
      }
    }
    #endif

    #region Nested types
    /// <summary>Container for nested types declared in the RenewAddressableLeaseResponseProto message type.</summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static partial class Types {
      public enum Status {
        [pbr::OriginalName("OK")] Ok = 0,
        [pbr::OriginalName("ERROR")] Error = 1,
      }

    }
    #endregion

  }

  public sealed partial class AbandonAddressableLeaseRequestProto : pb::IMessage<AbandonAddressableLeaseRequestProto>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<AbandonAddressableLeaseRequestProto> _parser = new pb::MessageParser<AbandonAddressableLeaseRequestProto>(() => new AbandonAddressableLeaseRequestProto());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<AbandonAddressableLeaseRequestProto> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Orbit.Shared.Proto.AddressableManagementReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AbandonAddressableLeaseRequestProto() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AbandonAddressableLeaseRequestProto(AbandonAddressableLeaseRequestProto other) : this() {
      reference_ = other.reference_ != null ? other.reference_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AbandonAddressableLeaseRequestProto Clone() {
      return new AbandonAddressableLeaseRequestProto(this);
    }

    /// <summary>Field number for the "reference" field.</summary>
    public const int ReferenceFieldNumber = 1;
    private global::Orbit.Shared.Proto.AddressableReferenceProto reference_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Orbit.Shared.Proto.AddressableReferenceProto Reference {
      get { return reference_; }
      set {
        reference_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as AbandonAddressableLeaseRequestProto);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(AbandonAddressableLeaseRequestProto other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Reference, other.Reference)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (reference_ != null) hash ^= Reference.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (reference_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Reference);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (reference_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Reference);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (reference_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Reference);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(AbandonAddressableLeaseRequestProto other) {
      if (other == null) {
        return;
      }
      if (other.reference_ != null) {
        if (reference_ == null) {
          Reference = new global::Orbit.Shared.Proto.AddressableReferenceProto();
        }
        Reference.MergeFrom(other.Reference);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (reference_ == null) {
              Reference = new global::Orbit.Shared.Proto.AddressableReferenceProto();
            }
            input.ReadMessage(Reference);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            if (reference_ == null) {
              Reference = new global::Orbit.Shared.Proto.AddressableReferenceProto();
            }
            input.ReadMessage(Reference);
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class AbandonAddressableLeaseResponseProto : pb::IMessage<AbandonAddressableLeaseResponseProto>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<AbandonAddressableLeaseResponseProto> _parser = new pb::MessageParser<AbandonAddressableLeaseResponseProto>(() => new AbandonAddressableLeaseResponseProto());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<AbandonAddressableLeaseResponseProto> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Orbit.Shared.Proto.AddressableManagementReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AbandonAddressableLeaseResponseProto() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AbandonAddressableLeaseResponseProto(AbandonAddressableLeaseResponseProto other) : this() {
      abandoned_ = other.abandoned_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public AbandonAddressableLeaseResponseProto Clone() {
      return new AbandonAddressableLeaseResponseProto(this);
    }

    /// <summary>Field number for the "abandoned" field.</summary>
    public const int AbandonedFieldNumber = 1;
    private bool abandoned_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Abandoned {
      get { return abandoned_; }
      set {
        abandoned_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as AbandonAddressableLeaseResponseProto);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(AbandonAddressableLeaseResponseProto other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Abandoned != other.Abandoned) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Abandoned != false) hash ^= Abandoned.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (Abandoned != false) {
        output.WriteRawTag(8);
        output.WriteBool(Abandoned);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (Abandoned != false) {
        output.WriteRawTag(8);
        output.WriteBool(Abandoned);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Abandoned != false) {
        size += 1 + 1;
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(AbandonAddressableLeaseResponseProto other) {
      if (other == null) {
        return;
      }
      if (other.Abandoned != false) {
        Abandoned = other.Abandoned;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Abandoned = input.ReadBool();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 8: {
            Abandoned = input.ReadBool();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code
