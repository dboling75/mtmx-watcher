# CAMT.053 to MT950 Translator System
## Requirements Document - Version 0.1
**Date:** 2025-10-03  
**Status:** Draft - Awaiting Approval  
**Document Type:** Quick Start / Minimum Viable Requirements

---

## 1. System Overview

### 1.1 Purpose
A .NET Console application that retrieves CAMT.053.001.08 messages from an Oracle database, extracts and translates them to MT950 format using a Swift Translator JAR, and routes the output to ReconNet via MessageWay.

### 1.2 Architecture
- **Type:** Monolithic .NET Console Application
- **Execution Model:** Run-on-demand or scheduled
- **External Dependencies:** 
  - Oracle Database
  - Swift Translator JAR (MessageTranslator)
  - Java Runtime Environment (for JAR execution)

---

## 2. Functional Requirements

### 2.1 Database Retrieval Phase

**FR-001: Database Connection**
- System will connect to an Oracle database
- Connection parameters to be configured (TBD: connection string format/location)

**FR-002: Incremental Message Retrieval**
- System will maintain a local tracking file storing the last processed `recordID`
- System will query for all messages WHERE `recordID > last_processed_recordID`
- System will order results by `recordID` (ascending implied)

**FR-003: Message Extraction to Inbox**
- For each record retrieved, system will:
  - Extract the XML message content
  - Generate a unique filename (TBD: naming convention)
  - Write the XML message to `/camt053-inbox` folder
  - Update the last processed `recordID` after successful write (TBD: update strategy - per message or batch?)

### 2.2 Envelope Extraction Phase

**FR-004: XML Envelope Removal**
- For each file in `/camt053-inbox`, system will:
  - Read the XML file
  - Extract the inner CAMT.053 message from the XML envelope
  - Write the extracted message to `/camt053-extracted`
  - Use same filename or generate new one (TBD: filename strategy)
  - Delete the original file from `/camt053-inbox` after successful extraction

### 2.3 Translation Phase

**FR-005: CAMT to MT950 Translation**
- For each file in `/camt053-extracted`, system will:
  - Execute the MessageTranslator JAR file as an external process
  - Pass the CAMT.053 file as input (TBD: JAR command-line syntax)
  - Capture the MT950 output
  - Write the result to `/mt950-out`
  - Use same filename or generate new one (TBD: filename and extension strategy)
  - Delete the original file from `/camt053-extracted` after successful translation

### 2.4 Routing Phase

**FR-006: MessageWay Routing**
- For each file in `/mt950-out`, system will:
  - Copy/write the file to `<MessageWayFolder>` (TBD: configuration parameter)
  - Log the file write operation (TBD: log format and destination)
  - Delete the file from `/mt950-out` after successful routing
  - Log the file deletion operation

---

## 3. Non-Functional Requirements

### 3.1 Data Management

**NFR-001: Folder Structure**
- System will create/verify existence of the following subfolders:
  - `/camt053-inbox`
  - `/camt053-extracted`
  - `/mt950-out`
- (TBD: Base path for these folders - application directory, configurable path?)

**NFR-002: State Persistence**
- System will maintain last processed `recordID` in a local file
- (TBD: File format - plain text, JSON, XML?)
- (TBD: File location and name)

### 3.2 Logging

**NFR-003: Logging Requirements**
- System will log files written to MessageWay
- System will log files deleted from `/mt950-out`
- (TBD: Additional logging - errors, database operations, translation results?)
- (TBD: Log destination - file, console, both?)
- (TBD: Log format and rotation strategy)

---

## 4. Technical Decisions Required (TBD Items)

### 4.1 Configuration
- [ ] Connection string storage and format for Oracle DB
- [ ] MessageWayFolder path configuration method
- [ ] Swift Translator JAR path configuration
- [ ] Base path for processing folders

### 4.2 Database Details
- [ ] Oracle table name and schema
- [ ] Table structure/column names
- [ ] RecordID data type and uniqueness guarantee
- [ ] Message content column name and data type
- [ ] Date stamp column (mentioned but not used - clarify if needed)

### 4.3 File Handling
- [ ] Filename generation strategy for each phase
- [ ] File extension conventions (.xml, .txt, custom?)
- [ ] Duplicate filename handling
- [ ] File encoding (UTF-8, UTF-16?)

### 4.4 Swift Translator Integration
- [ ] JAR file command-line interface syntax
- [ ] Input/output method (file path, stdin/stdout, temp files?)
- [ ] Error handling and return codes
- [ ] Java runtime version requirements

### 4.5 Error Handling
- [ ] Database connection failure behavior
- [ ] File I/O error handling strategy
- [ ] JAR execution failure handling
- [ ] Partial batch failure behavior (continue or abort?)
- [ ] Retry logic requirements

### 4.6 State Management
- [ ] When to update last processed recordID (per message, per batch, end of run?)
- [ ] Recovery strategy if process crashes mid-execution
- [ ] Handling of partially processed files in folders

### 4.7 Logging
- [ ] Log level requirements (Info, Debug, Error only?)
- [ ] Log file rotation and retention
- [ ] Structured logging format preference

---

## 5. Assumptions

1. Oracle database is accessible and credentials will be provided
2. Swift Translator JAR is available and functional
3. Java Runtime Environment is installed on target system
4. MessageWayFolder exists and is writable
5. Application has necessary file system permissions for all folders
6. Messages in database are well-formed CAMT.053.001.08 XML
7. Single-threaded processing is acceptable (no parallel processing required)

---

## 6. Out of Scope (for MVP)

- Multi-threaded or parallel processing
- Message validation or business rule checking
- Database write-back of processing status
- Web interface or API
- Real-time monitoring or alerting
- Message replay or reprocessing capabilities
- Advanced error recovery or retry mechanisms

---

## 7. Open Questions for Design Review

1. **Error Recovery:** If the process fails midway, how should it recover? Should partially processed messages be reprocessed?

2. **Transaction Boundaries:** Should the recordID tracking file be updated after each message or at the end of the entire batch?

3. **File Naming:** What naming convention should be used? Options:
   - Keep original DB recordID (e.g., `CAMT_12345.xml`)
   - Timestamp-based (e.g., `CAMT_20251003_143022_001.xml`)
   - GUID-based

4. **Envelope Structure:** Can you provide a sample of the XML envelope structure for extraction logic?

5. **MessageWay Requirements:** Does MessageWay require specific file naming, extensions, or metadata?

6. **Processing Frequency:** How often will this application run? (Scheduled task, manual, continuous monitoring?)

7. **Volume Expectations:** Expected message volume per run? (Affects logging verbosity and batch processing decisions)

8. **Date Stamp Usage:** The requirements mention a date stamp in the DB table - is this for information only, or should it factor into processing logic?

---

## 8. Next Steps

1. Review and approve/modify these requirements
2. Answer TBD items and open questions
3. Create detailed technical design document
4. Define configuration file format
5. Design error handling and logging strategy
6. Proceed to implementation

---

**Approval Section**

| Role | Name | Signature | Date |
|------|------|-----------|------|
| Product Owner | | | |
| Technical Lead | | | |
| QA Lead | | | |

---

*Document Version History*
- v0.1 (2025-10-03): Initial draft based on quick start requirements