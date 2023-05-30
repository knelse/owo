using System.Collections.Generic;
using RogueGenesia.Data;

namespace OwO.Pets;

public class NudeRogPet : PetData
{
    
    public override string GetName()
    {
        return OwOMod.Owofy(OwOMod.ProcessAvatars("Lil' Roggie"));
    }

    public override string GetDescription()
    {
        return OwOMod.Owofy(OwOMod.ProcessAvatars("Black magic is great for many things.\n\n" +
                                                  "Rog's trusty stress relief sock™ was not one of them."));
    }

    public override List<PetBehaviour> GetPetBehaviours()
    {
        return new List<PetBehaviour>
        {
            new MoveToADifferentPositionPetBehaviour(),
        };
    }
}