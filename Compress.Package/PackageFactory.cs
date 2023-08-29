using Compress.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Compress.Package
{
    public class PackageFactory
    {
        public PackageFactory()
        {
            this.serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IFileSystem, FileSystem>();
            serviceCollection.AddSingleton<IPackageWriter, PackageWriter>();
            serviceCollection.AddSingleton<IPackageExtractor, PackageExtractor>();
            this.SetCryptoFactory(new LzwFactory());
            serviceCollection.AddTransient<PackageFile>();
            
        }
        public PackageFile CreatePackage()
        {
            //return new PackageFile(FileSystem, new PackageWriter(FileSystem, Packer), new PackageExtractor(FileSystem, Unpacker));
            var service = this.serviceCollection.BuildServiceProvider();
            return service.GetRequiredService<PackageFile>();
        }

        public void SetCryptoFactory(ICryptoFactory packerFactory)
        {
            serviceCollection.AddSingleton<ICryptoFactory>(packerFactory);
        }

        private ServiceCollection serviceCollection;
    }
}
