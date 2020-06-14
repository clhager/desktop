package io

import java.io.File

class Write() {
    companion object {
        fun writeToFile(path: String, text: String) = File(path).writeText(text)
    }
}