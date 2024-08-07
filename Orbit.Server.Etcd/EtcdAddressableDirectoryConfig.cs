using Grpc.Core;
using Grpc.Net.Client;
using Orbit.Server.Mesh;
using Orbit.Util.Di;

namespace Orbit.Server.Etcd;

public class EtcdAddressableDirectoryConfig : ExternallyConfigured<IAddressableDirectory>
{
    public string Url { get; set; } = Environment.GetEnvironmentVariable("ADDRESSABLE_DIRECTORY") ?? "0.0.0.0";

    public override Type InstanceType => typeof(EtcdAddressableDirectory);

    public static GrpcChannel CreateAuthenticatedChannel(string address)
    {
        var credentials = CallCredentials.FromInterceptor((context, metadata) =>
        {
            // if (!string.IsNullOrEmpty(_token))
            // {
            //     metadata.Add("Authorization", $"Bearer {_token}");
            // }
            return Task.CompletedTask;
        });

        var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.Create(ChannelCredentials.Insecure, credentials),
            UnsafeUseInsecureChannelCallCredentials = true
        });
        return channel;
    }
}