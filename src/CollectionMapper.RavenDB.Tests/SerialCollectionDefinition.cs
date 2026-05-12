using Xunit;

namespace CollectionMapper.RavenDB.Tests;

[CollectionDefinition("Serial", DisableParallelization = true)]
public class SerialCollectionDefinition { }
