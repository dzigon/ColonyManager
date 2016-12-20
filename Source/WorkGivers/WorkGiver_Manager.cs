﻿// Karel Kroeze
// WorkGiver_Manager.cs
// 2016-12-09

using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace FluffyManager
{
    internal class WorkGiver_Manage : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode
        {
            get { return PathEndMode.InteractionCell; }
        }

        public override ThingRequest PotentialWorkThingRequest
        {
            get { return ThingRequest.ForGroup( ThingRequestGroup.PotentialBillGiver ); }
        }

        public override bool HasJobOnThing( Pawn pawn, Thing t )
        {
#if DEBUG_WORKGIVER
            Log.Message( "Checking " + t.LabelCap + " for job." );
            Log.Message( "ManagerStation" + ( t as Building_ManagerStation != null ) );
            Log.Message( "Comp" + ( t.TryGetComp<Comp_ManagerStation>() != null ) );
            Log.Message( "Incap" + ( !pawn.Dead && !pawn.Downed && !pawn.IsBurning() && !t.IsBurning() ) );
            Log.Message( "CanReserve and reach" + pawn.CanReserveAndReach( t, PathEndMode, Danger.Some ) );
            var powera = t.TryGetComp<CompPowerTrader>();
            Log.Message( "Power" + ( powera == null || powera.PowerOn ) );
            Log.Message( "Job" + ( Manager.For( pawn.Map ).JobStack.NextJob != null ) );
#endif
            if ( !( t is Building_ManagerStation ) )
            {
                return false;
            }

            if ( t.TryGetComp<Comp_ManagerStation>() == null )
            {
                return false;
            }

            if ( pawn.Dead ||
                 pawn.Downed ||
                 pawn.IsBurning() ||
                 t.IsBurning() )
            {
                return false;
            }

            if ( !pawn.CanReserveAndReach( t, PathEndMode, Danger.Some ) )
            {
                return false;
            }

            var power = t.TryGetComp<CompPowerTrader>();
            if ( power != null &&
                 !power.PowerOn )
            {
                return false;
            }

            if ( Manager.For( pawn.Map ).JobStack.NextJob != null )
            {
                return true;
            }

            return false;
        }

        public override Job JobOnThing( Pawn pawn, Thing t )
        {
            return new Job( DefDatabase<JobDef>.GetNamed( "ManagingAtManagingStation" ), t as Building_ManagerStation );
        }

        public override IEnumerable<Thing> PotentialWorkThingsGlobal( Pawn pawn )
        {
            return pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Building_ManagerStation>()
                       .Select( b => b as Thing );
        }
    }
}
