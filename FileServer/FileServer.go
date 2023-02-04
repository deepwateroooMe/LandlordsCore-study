package main

import (
	"log"
	"net/http"
)
 // 这个是go语言的进程启动程序，我可以暂时不学这个
func main() {
	// Simple static webserver:
	log.Fatal(http.ListenAndServe(":8080", http.FileServer(http.Dir("../Release/"))))
}
