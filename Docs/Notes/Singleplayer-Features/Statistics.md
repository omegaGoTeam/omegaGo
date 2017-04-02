# Statistics

We store statistics _locally_, not in the _roaming_ storage, because that's common practice in UWP apps (because of the threat of storage overwriting).

All potentially interesting information is kept tracked of, except for a breakdown of games against specific AI programs with specific difficulty and handicap settings. This breakdown is not kept track of because it would need additional UI (more work for us) and it's not too useful anyway.