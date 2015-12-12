# SOCVR Slack Stat Collector Functional Specifications


The bot has two main commands:

    @bot day-stats [filter] <date> [<hourStart>-<hourEnd>] [output-type]
    @bot range-stats <start-date> <end-date>

Unless specified, stat collection will not include messages that have been moved to other chat rooms.

## Day Stats
This command is used for finding message stats for a given day.

### Filter
By default the result will include all columns. The user may include one of the following phrases. By adding a filter the output summary will reflect only that information and the output table or csv (if outputed) will contain only those columns.

|  Filter   |                                                                Description                                                                 |                                                                       Summary Message                                                                        |
|-----------|--------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------|
| totals    | Shows how many messages each person has posted that day. This will include messages moved to the graveyard.                                | A total of `<total-messages>` messages were posted on `<date>` between hours `<start-hour>` and `<end-hour>`."                                               |
| cv-pls    | Shows how many formal close vote requests the user has posted. This includes messages moved to the CV-Graveyard room.                      | A total of `<total-cv-pls>` close requests were posted on `<date>` between hours `<start-hour>` and `<end-hour>`."                                           |
| links     | Shows how many messages a person has posted which contain links.                                                                           | A total of `<total-messages>` link messages (not cv-pls, not images, not one-box) were posted on `<date>` between hours `<start-hour>` and `<end-hour>`."    |
| one-boxed | Shows how many messages the user has posted that are one-boxed (and not images).                                                           | A total of `<total-messages>` one-box messages (not images) were posted on `<date>` between hours `<start-hour>` and `<end-hour>`."                          |
| stars     | Shows two columns of data: the number of messages the user made that were starred, and the total number of stars that user got in the day. | A total of `<total-starred-messages>` messages were starred and `<total-stars>` stars were given on `<date>` between hours `<start-hour>` and `<end-hour>`." |

If a filter is not specified, all columns are returned and the summary reflects only totals information.

## Date
The date must be specified in ISO format (`yyyy-MM-dd`) or use a shortcut ("today", "yesterday", "X days ago"). This is required.


## Hour Range
For single day lookups, you can specify a particular hour range. Valid numbers are 0 to 24. For example:

```
0-24
05-16
9-20
```

## Output Type
In addition to the summary information, more information can be outputed in one of the following ways.

- `summary-only`
- `table`
- `csv`

### Summary Only
No additional information besides the summary will be displayed. This is the default behavior if the output type is not specified.

### Table
An ascii table of the values will be posted to chat. An example:

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

The table will be ordered by the most important column, which is specified by the filter. If no filter is given, the table will be ordered by Total Messages descending.

### CSV
A csv file will be uploaded with the summary information containing the requested information. The data will be ordered by the most important column, which is specified by the filter. If no filter is given, the data will be ordered by Total Messages descending.


## Range Stats
This command is used for getting stats between two dates. This command does not have different output types. A CSV file will always be uploaded for this command.

Because the bot will take time to collect the stat, and assuming the inputs are valid, the bot will first reply with:

> Collecting stats between <start-date> and <end-date>. This may take some time.

Once the bot has finished collecting stats, it will output the summary and csv information as normal.
