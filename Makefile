
stop:
	docker stop Identity

build_run: 
	docker build -t identity . && \
		docker run -d -ti --rm --name Identity -p 8080:80 identity