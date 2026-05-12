using CollectionMapper.RavenDB.Attributes;

namespace CollectionMapper.RavenDB.Tests.Fixtures;

[RavenCollection("Fruits")]
public class Fruit { }

[RavenCollectionAssignedFrom<Fruit>]
public class Apple : Fruit { }

[RavenCollectionAssignedFrom<Fruit>]
public class Grape : Fruit { }

public class UnmappedDocument { }

public class Banana : Fruit { }

public class VehicleBase { }

public class VehicleWithMotor : VehicleBase { }

[RavenCollectionAssignedFrom<VehicleBase>]
public class Car : VehicleWithMotor { }

public class BananaCollectionMapper : RavenDBCollectionMapper
{
    public BananaCollectionMapper()
    {
        Map<Banana>("ScannedBananas");
    }
}
