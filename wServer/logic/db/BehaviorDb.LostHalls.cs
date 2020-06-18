using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wServer.logic.behaviors;
using wServer.logic.transitions;

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private _ LostHalls = () => Behav()
        #region Marble Colossus
           .Init("LH Marble Colossus",
            new State(
                new State("Idle",
                    new PlayerWithinTransition(50, "Phase 1")

                ),
                new State("Phase 1",
                    new Taunt("Look upon my mighty bulwark."),
                    new Shoot(10, 12, shootAngle: 30, projectileIndex: 0, fixedAngle: 30, angleOffset: 30, defaultAngle: 30, predictive: 0, coolDown: new Cooldown(3000, 0)),
                    new Shoot(10, 12, shootAngle: 30, projectileIndex: 0, fixedAngle: 15, predictive: 0, coolDownOffset: 1500, coolDown: new Cooldown(3000, 0)),
                    new ConditionalEffect(ConditionEffectIndex.Armored),
                    new HpLessTransition(0.95, "Phase 2"),
                    new TimedTransition(1000, "Phase 2", false)//10000

                ),
                new State("Phase 2",
                    new Taunt("You doubt my strength? FATUUS! I will destroy you!"),
                    new Follow(1, 15, 2, 5000, new Cooldown(0, 0)),
                    new Shoot(5, 5, projectileIndex: 1, predictive: 0, shootAngle: 6, coolDown: new Cooldown(3000, 0)),
                    new Shoot(20, 8, projectileIndex: 2, fixedAngle: 0, shootAngle: 45, coolDownOffset: 250, coolDown: new Cooldown(3000, 0)),
                    new HpLessTransition(.9, "Phase 2 to Phase 3"),
                    new TimedTransition(5000, "Phase 2 to Phase 3", false)//20000
                ),
                new State("Phase 2 to Phase 3",
                    new Taunt("I cast you off!"),
                    new MoveTo(70, 69, 2, isMapPosition: true, instant: false),
                    new TimedTransition(2000, "Phase 3", false)
                    ),
                new State("Phase 3",
                    new MoveTo(70, 69, 2, isMapPosition: true, instant: true),
                    new ConditionalEffect(ConditionEffectIndex.Armored),
                    new Spawn("LH Colossus Rock 1", 1, 1, coolDown: new Cooldown(5000, 0)),
                    new Spawn("LH Colossus Rock 2", 1, 1, coolDown: new Cooldown(5000, 0)),
                    new Spawn("LH Colossus Rock 3", 1, 1, coolDown: new Cooldown(5000, 0)),
                    new Spawn("LH Colossus Rock 4", new Cooldown(5000, 0), 1, 0, CoolDownOffset: -2500),
                    new Spawn("LH Colossus Rock 5", new Cooldown(5000, 0), 1, 0, CoolDownOffset: -2500),
                    new Spawn("LH Colossus Rock 6", new Cooldown(5000, 0), 1, 0, CoolDownOffset: -2500)
                    )
            )
           )
        #endregion
        #region LH Colossus Rocks
            .Init("LH Colossus Rock 1",
                new State(
                    new State("Idle",
                        new MoveTo(5, 0, 0.4, true, false, false),
                        new TimedTransition(2000, "Arrival", false)
                    ),
                    new State("Arrival",
                        new Shoot(6, 8, shootAngle: 45, projectileIndex: 0, fixedAngle: 0),
                        new Suicide()
                        )
                )
            )
        .Init("LH Colossus Rock 2",
                new State(
                    new State("Idle",
                        new MoveTo(-3, 3, 0.4, true, false, false),
                        new TimedTransition(2000, "Arrival", false)
                    ),
                    new State("Arrival",
                        new Shoot(6, 8, shootAngle: 45, projectileIndex: 0, fixedAngle: 0),
                        new Suicide()
                        )
                )
            )
        .Init("LH Colossus Rock 3",
                new State(
                    new State("Idle",
                        new MoveTo(-3, -3, 0.4, true, false, false),
                        new TimedTransition(2000, "Arrival", false)
                    ),
                    new State("Arrival",
                        new Shoot(6, 8, shootAngle: 45, projectileIndex: 0, fixedAngle: 0),
                        new Suicide()
                        )
                )
            )
        .Init("LH Colossus Rock 4",
                new State(
                    new State("Idle",
                        new MoveTo(-5, 0, 0.4, true, false, false),
                        new TimedTransition(2000, "Arrival", false)
                    ),
                    new State("Arrival",
                        new Shoot(6, 8, shootAngle: 45, projectileIndex: 0, fixedAngle: 0),
                        new Suicide()
                        )
                )
            )
        .Init("LH Colossus Rock 5",
                new State(
                    new State("Idle",
                        new MoveTo(3, -3, 0.4, true, false, false),
                        new TimedTransition(2000, "Arrival", false)
                    ),
                    new State("Arrival",
                        new Shoot(6, 8, shootAngle: 45, projectileIndex: 0, fixedAngle: 0),
                        new Suicide()
                        )
                )
            )
        .Init("LH Colossus Rock 6",
                new State(
                    new State("Idle",
                        new MoveTo(3,3, 0.4, true, false, false),
                        new TimedTransition(2000, "Arrival", false)
                    ),
                    new State("Arrival",
                        new Shoot(6, 8, shootAngle: 45, projectileIndex: 0, fixedAngle: 0),
                        new Suicide()
                        )
                )
            );
        #endregion
    }
}
