ROOT:=$(shell pwd)
PROJECT=Mitto
SUBMODULES= $(wildcard src/Lib/*)
BUILDDIR=build

CONFIGURATION=Debug
VERSION=$(shell cat VERSION)
REVISION=$(shell git rev-parse --short HEAD)

rpm: clean tar rpmbuild

build: clean app

clean:
	rm -rf $(ROOT)/bin
	rm -rf $(ROOT)/build
	rm -rf $(ROOT)/*/bin/
	rm -rf $(ROOT)/*/obj/
	#rm -rf packages

tar:
	tar -ccvpf $(PROJECT).tgz . --exclude=*.tgz

rpmbuild:
	mkdir -p ~/rpmbuild/{BUILD,RPMS,SOURCES,SPECS,SRPMS}
	echo '%_topdir %(echo ~)/rpmbuild' > ~/.rpmmacros
	
	cp $(PROJECT).tgz ~/rpmbuild/SOURCES/
	cp autorender-server.spec ~/rpmbuild/SPECS/
	
	cd ~ && rpmbuild -ba ~/rpmbuild/SPECS/autorender-server.spec --define="_version $(VERSION)" --define="_revision $(REVISION)"

submodules:
	$(foreach MODULE,$(SUBMODULES), cd $(ROOT)/$(MODULE) && make build ;)

app: submodules
	nuget restore $(ROOT)/src/$(PROJECT).sln
	msbuild $(ROOT)/src/$(PROJECT).sln /t:$(PROJECT) /p:Configuration="$(CONFIGURATION)" /p:Platform="Any CPU" /p:BuildProjectReferences=false
