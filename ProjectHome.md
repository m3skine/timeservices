# Time Services #

Time Services is a micro-framework providing real-time simulation across many objects over a time-line. Implementation has been optimized for objects with deferred execution and propagation delays, allocating execution of other objects across those delays. Both CPU and GPU implementations, and sub-systems to handle data across in-congruent time-domains are provided.

In real life all things function and interact at the exact same time. However the parallel execution capabilities of a CPU or even GPU are limiting. In order to process hundreds or even thousands of operations at the same time concessions must be made.

Processing must run in a separate time domain. And two states must be maintained, one representing information as it last was, and the second representing information as it will. Evaluations read from the first and write to the second. All concurrent operations for a single time are evaluated then their states are flipped and the time-line is advanced one unit, effectively progressing time.

Systems Involved:
  * TimeEngine - coordinates objects states, and schedules their concurrent execution.
  * TimeStation - handles data across in-congruent time-domains.

