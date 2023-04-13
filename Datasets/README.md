# Datasets

The files in this directory are used to determine, which load will be simulated on the managed devices at a given point in time. This load will then be distributed over the amount of devices that are currently active, which is the central variable of the system, because it basically is the only variable adjusted by the system.

EACH dataset correspondes to its own experiment, every dataset creates its own individual scenario.

Each line in every dataset has the following structure:
- `id`: The timestamp of the data point. Timestamps do not represent seconds, they are just a number that is incremented by one for each data point. This is done to make the data more readable and the simulation easier reproduceable.
- `CPU`: The total CPU workload on the system. This total CPU workload is then distributed over the amount of devices that are currently active. Every device can handle up to 1 CPU workload, which means that 5 CPU workload distributed across 10 devices means that every device has 0.5 CPU workload.
- `Memory`: Exactly the same as the CPU workload, but for memory.
- `Availability`: The average availability of each and every device individually. This is a value between 0 and 1, where 0 means that the device is completely unavailable and 1 means that the device is completely available. This value is then used to determine the total availability of the system, which can be computed by $AV_{total} = 1 - (1 - AV_{avg})^N$, where N is the number of devices (=replicas) in the system. This means, that the total availability increases both with the average availability of the devices and the number of devices. This is a very simple model, but it is sufficient for the proof-of-concept.