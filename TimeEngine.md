# Introduction #

The Time-Engine coordinates objects states, and schedules their concurrent execution along a time-line.


## Theoretical Implementation ##
Ideally the Time-Engine would be implemented as an infinite time-line representing every moment now and into the future. Each time-line entry as a list of items to be evaluated for that time. With a method to place items on the time-line in the correct spot. And finally a method to evaluate all items in the first time-line entry list, advance item states, then advance the time-line to the next time unit.

## Practical Implementation ##
`[Text Needed]`

<br />
**Need to format and add addition information. Stream of consciousness follows:**

---

  * Time-line fixed slots size with a rolling zero and an overflow which hibernates on adds and de-hibernates on rolling zero's roll.
  * factional time slot using hard entries for whole part and hash sub-lookups for factional part.
  * whole and fractional part implemented as fixed position, efficient implementation. requires base conversion methods to move from base-10 to base-N.
    * precision generated at compile time, need to parametrize precision using code emit.
  * two types of objects, single executive and multi-executive. can use link list for single executive to decrease memory fragmentation.
  * batched executive per-thread/worker.
  * isolated pending operations per-thread/worker during time processing with apply at thread join. lower lock contention.