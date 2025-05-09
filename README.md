# MX/MT Adapter Project

This project will perform translaton for certain MT->MX transforms.

## Components

- QueueBuilder
- MessageQueue: Moc an in mem version of the ServiceBus and interact with it as a service
  - /queues/{queuename}
  - POST /queues/{queuename} - Add an item
  - GET  /queues/{queuename}/{itemId}?dequeue=true - Get an item (remove from queue, dequeue=true)
  - GET  /queues/{queuename}/ - Get list of current items (leave them on the queue)
- MTMX Transform API
  - POST /transform/MT103ToPACS008
  - POST /transform/MT202ToPACS009
  - POST /transform/MT192ToCAMT056
  - POST /transform/MT292ToCAMT029
  - POST /transform/CAMT053ToMT950 

## Messages to Transform

| **MT Message (FIN)** | **MX Equivalent (CBPR+)**     | **Description**                                                                 |
|----------------------|-------------------------------|---------------------------------------------------------------------------------|
| **MT103**            | `pacs.008.001.08`             | **Single customer credit transfer** — used to instruct a payment from one customer to another across banks. |
| **MT202**            | `pacs.009.001.08`             | **Financial institution transfer** — for bank-to-bank fund transfers (no customer involvement).             |
| **MT192**            | `camt.056.001.08`             | **Request for cancellation** — used to request the cancellation of a previously sent payment.              |
| **MT292**            | `camt.029.001.09`             | **Request for investigation response** — used to respond to queries (e.g., exceptions or investigations).   |

