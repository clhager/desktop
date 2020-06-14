package io

import main.World
import java.io.File

class Read2 {
    companion object {
        private val tokenDividers = mutableListOf<Char>('=', '{', '}', '[', ']', '(', ')', ',')
        private val charactersAllowed = mutableListOf<Char>().apply {
            addAll('a'..'z')
            addAll('A'..'Z')
            addAll('0'..'9')
            addAll(tokenDividers)
        }

        fun readFromFile(path: String): World {
            val file = File(path)
            if (file.exists().not() or file.isFile.not()) throw IllegalArgumentException("File $path does not exist")

            val fileText = File(path).readText()
            val filteredText = fileText.filter { it in Read2.charactersAllowed }
            val tokens = filteredText.tokenize(Read2.tokenDividers)
            val mapifiedTokens = Read.mapify(tokens)

            return World()
        }

        private fun String.tokenize(dividers: List<Char>): List<String> {
            val tokens = mutableListOf<String>()
            val currentString = mutableListOf<Char>()
            for (char: Char in this) {
                if (char in dividers) {
                    if (currentString.size > 0) tokens.add(currentString.joinToString(""))
                    currentString.clear()
                    tokens.add(char.toString())
                } else {
                    currentString.add(char)
                }
            }
            return tokens.toList()
        }



    }
}