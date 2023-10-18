# FileCompressor in C#

A flexible and modifiable FileCompressor written in C#. This document provides an overview of the supported commands and their applications.

---

## Table of Contents

- [Commands](#commands)
- [Usage Examples](#usage-examples)

---

## Commands

Commands are provided as arguments when executing the FileCompressor. They enable seamless archive management.

> **Note**: Commands can be chained together.

- **Create a New Archive**  
  `-c` or `--create`  
  Creates a new archive. Requires `-s` and `-d`.

- **Append to an Existing Archive**  
  `-a` or `--append`  
  Adds new contents to an existing archive. Requires `-s` and `-d`.

- **Extract from an Archive**  
  `-x` or `--extract`  
  Extracts contents from an archive. Requires `-s` and `-d`.

- **Compress with RLE**  
  `-rle` or `--rleCompress`  
  Compresses files using RLE before adding to the archive.

- **Display Archive Information**  
  `-i` or `--info`  
  Shows meta-information of an archive. Requires `-s`.

- **List Archive Contents**  
  `-l` or `--list`  
  Lists the filenames within an archive. Requires `-s`.

- **Retry Configuration**  
  `-r` or `--retry`  
  Defines the retry attempts for failed file access. Example: `-r 3`

- **Wait Between Retries**  
  `-w` or `--wait`  
  Configures wait time between retries. Example: `-w 5`

- **Specify Source**  
  `-s` or `--source`  
  Determines the source for operations.

- **Specify Destination**  
  `-d` or `--destination`  
  Determines the destination for operations.

---

## Usage Examples

Below are practical examples demonstrating how to utilize the FileCompressor.

### 1. Create a New Archive
**Command:**  

-c -s [Source Directory] -d [Destination File]

**Explanation:**  
This command creates a new archive by taking all the files from the specified source directory and packaging them into the destination file.

### 2. Create Two Archives with RLE Compression
**Command:**  
-c -s [Source Directory] -d [First Archive] -rle -c -s [Source Directory] -d [Second Archive] -rle


**Explanation:**  
This command sequence creates two separate archives from the same source directory. Both archives use RLE compression for their contents.

### 3. Create an Archive with RLE Compression and Specific Retry and Wait Settings
**Command:**  
-c -s [Source Directory] -d [Destination Archive] -rle -w 5 -r 5

**Explanation:**  
Creates an archive with RLE compression. If there's a failure during the creation process, the command will attempt to retry up to 5 times, waiting for 5 seconds between each attempt.

### 4. Append to an Existing Archive with RLE Compression
**Command:**  
-a -s [Source Directory] -d [Destination Archive] -rle

**Explanation:**  
This command appends new files from the source directory to an existing archive using RLE compression.

### 5. Extract from an Archive and Create a New Directory
**Command:**  
-x -s [Source Archive] -d [Destination Directory]

**Explanation:**  
This extracts all files from the source archive to the destination directory.

### 6. Show Information About an Archive
**Command:**  
-i -s [Archive File]

**Explanation:**  
Displays meta-information about the specified archive.

### 7. List Content of an Archive
**Command:**  
-l -s [Archive File]

**Explanation:**  
Lists all file names stored in the archive.

### 8. Create Archive with Retry and Wait Settings
**Command:**  
-c -s [Source Directory] -d [Destination Archive] -r 3 -w 5

**Explanation:**  
Creates an archive from the specified directory. If there's an issue during t

