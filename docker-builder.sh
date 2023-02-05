if [ $# -eq 0 ]; then
    >&2 echo "No tag provided"
    exit 1
fi

set -ex

IMAGE_NAME="haumea/fireflyiii-pp"
TAG="${1}"

docker build -t ${IMAGE_NAME}:${TAG} -t ${IMAGE_NAME}:latest .
docker push ${IMAGE_NAME}:${TAG}
docker push ${IMAGE_NAME}:latest