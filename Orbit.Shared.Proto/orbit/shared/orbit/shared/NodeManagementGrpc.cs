// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: orbit/shared/node_management.proto
// </auto-generated>
#pragma warning disable 0414, 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Orbit.Shared.Proto {
  public static partial class NodeManagement
  {
    static readonly string __ServiceName = "Orbit.Shared.NodeManagement";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Orbit.Shared.Proto.JoinClusterRequestProto> __Marshaller_Orbit_Shared_JoinClusterRequestProto = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Orbit.Shared.Proto.JoinClusterRequestProto.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Orbit.Shared.Proto.NodeLeaseResponseProto> __Marshaller_Orbit_Shared_NodeLeaseResponseProto = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Orbit.Shared.Proto.NodeLeaseResponseProto.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Orbit.Shared.Proto.RenewNodeLeaseRequestProto> __Marshaller_Orbit_Shared_RenewNodeLeaseRequestProto = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Orbit.Shared.Proto.RenewNodeLeaseRequestProto.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::Orbit.Shared.Proto.LeaveClusterRequestProto> __Marshaller_Orbit_Shared_LeaveClusterRequestProto = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::Orbit.Shared.Proto.LeaveClusterRequestProto.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Orbit.Shared.Proto.JoinClusterRequestProto, global::Orbit.Shared.Proto.NodeLeaseResponseProto> __Method_JoinCluster = new grpc::Method<global::Orbit.Shared.Proto.JoinClusterRequestProto, global::Orbit.Shared.Proto.NodeLeaseResponseProto>(
        grpc::MethodType.Unary,
        __ServiceName,
        "JoinCluster",
        __Marshaller_Orbit_Shared_JoinClusterRequestProto,
        __Marshaller_Orbit_Shared_NodeLeaseResponseProto);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Orbit.Shared.Proto.RenewNodeLeaseRequestProto, global::Orbit.Shared.Proto.NodeLeaseResponseProto> __Method_RenewLease = new grpc::Method<global::Orbit.Shared.Proto.RenewNodeLeaseRequestProto, global::Orbit.Shared.Proto.NodeLeaseResponseProto>(
        grpc::MethodType.Unary,
        __ServiceName,
        "RenewLease",
        __Marshaller_Orbit_Shared_RenewNodeLeaseRequestProto,
        __Marshaller_Orbit_Shared_NodeLeaseResponseProto);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::Orbit.Shared.Proto.LeaveClusterRequestProto, global::Orbit.Shared.Proto.NodeLeaseResponseProto> __Method_LeaveCluster = new grpc::Method<global::Orbit.Shared.Proto.LeaveClusterRequestProto, global::Orbit.Shared.Proto.NodeLeaseResponseProto>(
        grpc::MethodType.Unary,
        __ServiceName,
        "LeaveCluster",
        __Marshaller_Orbit_Shared_LeaveClusterRequestProto,
        __Marshaller_Orbit_Shared_NodeLeaseResponseProto);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Orbit.Shared.Proto.NodeManagementReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of NodeManagement</summary>
    [grpc::BindServiceMethod(typeof(NodeManagement), "BindService")]
    public abstract partial class NodeManagementBase
    {
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::Orbit.Shared.Proto.NodeLeaseResponseProto> JoinCluster(global::Orbit.Shared.Proto.JoinClusterRequestProto request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::Orbit.Shared.Proto.NodeLeaseResponseProto> RenewLease(global::Orbit.Shared.Proto.RenewNodeLeaseRequestProto request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::Orbit.Shared.Proto.NodeLeaseResponseProto> LeaveCluster(global::Orbit.Shared.Proto.LeaveClusterRequestProto request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for NodeManagement</summary>
    public partial class NodeManagementClient : grpc::ClientBase<NodeManagementClient>
    {
      /// <summary>Creates a new client for NodeManagement</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public NodeManagementClient(grpc::ChannelBase channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for NodeManagement that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public NodeManagementClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected NodeManagementClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected NodeManagementClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Orbit.Shared.Proto.NodeLeaseResponseProto JoinCluster(global::Orbit.Shared.Proto.JoinClusterRequestProto request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return JoinCluster(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Orbit.Shared.Proto.NodeLeaseResponseProto JoinCluster(global::Orbit.Shared.Proto.JoinClusterRequestProto request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_JoinCluster, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Orbit.Shared.Proto.NodeLeaseResponseProto> JoinClusterAsync(global::Orbit.Shared.Proto.JoinClusterRequestProto request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return JoinClusterAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Orbit.Shared.Proto.NodeLeaseResponseProto> JoinClusterAsync(global::Orbit.Shared.Proto.JoinClusterRequestProto request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_JoinCluster, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Orbit.Shared.Proto.NodeLeaseResponseProto RenewLease(global::Orbit.Shared.Proto.RenewNodeLeaseRequestProto request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return RenewLease(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Orbit.Shared.Proto.NodeLeaseResponseProto RenewLease(global::Orbit.Shared.Proto.RenewNodeLeaseRequestProto request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_RenewLease, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Orbit.Shared.Proto.NodeLeaseResponseProto> RenewLeaseAsync(global::Orbit.Shared.Proto.RenewNodeLeaseRequestProto request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return RenewLeaseAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Orbit.Shared.Proto.NodeLeaseResponseProto> RenewLeaseAsync(global::Orbit.Shared.Proto.RenewNodeLeaseRequestProto request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_RenewLease, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Orbit.Shared.Proto.NodeLeaseResponseProto LeaveCluster(global::Orbit.Shared.Proto.LeaveClusterRequestProto request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return LeaveCluster(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::Orbit.Shared.Proto.NodeLeaseResponseProto LeaveCluster(global::Orbit.Shared.Proto.LeaveClusterRequestProto request, grpc::CallOptions options)
      {
        return CallInvoker.BlockingUnaryCall(__Method_LeaveCluster, null, options, request);
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Orbit.Shared.Proto.NodeLeaseResponseProto> LeaveClusterAsync(global::Orbit.Shared.Proto.LeaveClusterRequestProto request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return LeaveClusterAsync(request, new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual grpc::AsyncUnaryCall<global::Orbit.Shared.Proto.NodeLeaseResponseProto> LeaveClusterAsync(global::Orbit.Shared.Proto.LeaveClusterRequestProto request, grpc::CallOptions options)
      {
        return CallInvoker.AsyncUnaryCall(__Method_LeaveCluster, null, options, request);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      protected override NodeManagementClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new NodeManagementClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static grpc::ServerServiceDefinition BindService(NodeManagementBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_JoinCluster, serviceImpl.JoinCluster)
          .AddMethod(__Method_RenewLease, serviceImpl.RenewLease)
          .AddMethod(__Method_LeaveCluster, serviceImpl.LeaveCluster).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the  service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static void BindService(grpc::ServiceBinderBase serviceBinder, NodeManagementBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_JoinCluster, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Orbit.Shared.Proto.JoinClusterRequestProto, global::Orbit.Shared.Proto.NodeLeaseResponseProto>(serviceImpl.JoinCluster));
      serviceBinder.AddMethod(__Method_RenewLease, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Orbit.Shared.Proto.RenewNodeLeaseRequestProto, global::Orbit.Shared.Proto.NodeLeaseResponseProto>(serviceImpl.RenewLease));
      serviceBinder.AddMethod(__Method_LeaveCluster, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::Orbit.Shared.Proto.LeaveClusterRequestProto, global::Orbit.Shared.Proto.NodeLeaseResponseProto>(serviceImpl.LeaveCluster));
    }

  }
}
#endregion