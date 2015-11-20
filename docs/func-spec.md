make a bot for the slack room:

a user will ask the chatbot for stats about the types of messages posted on a particular UTC day.

    @bot stats [totals | cv-pls | links | moved | one-box | stars | starred | stars-in] <date> [<hourStart>-<hourEnd>]

Not including a specific column will show an overview table of all stats, ordered by "total messages" desc

the user can add a keyword to only show data about a single column:

- totals - shows how many messages each person has posted that day. This will include messages moved to any room
- cv-pls - shows how many formal close vote requests the user has posted. This will only detect messages with cv-pls, cv-plz, or "close" in the message in addition to a link. This includes messages moved to the CV-Graveyard room.
- links - shows how many messages a person has posted which contain links
- moved - shows how many messages the user has moved to different rooms
- one-box - shows how many messages the user has posted that are one-boxed (and not images)
- stars - shows two columns of data: the number of messages the user made that were starred, and the total number of stars that user got in the day.
- starred - shows only the first column of data from "stars"
- stars-in - shows only the second column of data from "starts"

Unless specified, stat collection will not include messages that have been moved to other chat rooms.

```
+--------+----------------+--------+-------+--------+-----------+---------+--------------+
|  User  | Total Messages | cv-pls | Links | Images | One-Boxed | Starred | Stars Gained |
+--------+----------------+--------+-------+--------+-----------+---------+--------------+
| User 1 |             34 |     10 |    50 |      2 |        10 |       1 |            4 |
| User 2 |             24 |     30 |    20 |      5 |         3 |       4 |           10 |
| User 3 |             18 |      0 |    11 |      1 |         5 |       6 |           22 |
| Total  |             76 |     40 |    81 |      8 |        18 |      11 |           36 |
+--------+----------------+--------+-------+--------+-----------+---------+--------------+
```

For `date`, you can type in a date in ISO format (`yyyy-MM-dd`) or use a shortcut ("today", "yesterday", "X days ago").
