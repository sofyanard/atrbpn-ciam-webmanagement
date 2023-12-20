node('base') {
	def appName="web-management"
	def projectName="webmanagement-dev"
    
	def gitBranch="main"

    stage('Clone') {
            sh "git config --global http.sslVerify false"
            withCredentials([usernamePassword(credentialsId: 'gitlab-token', usernameVariable: 'USERNAME', passwordVariable: 'PASSWORD')]) {
            sh "git clone -b ${gitBranch} https://\${USERNAME}:\${PASSWORD}@github.com/sofyanard/atrbpn-ciam-webmanagement.git source "
        }
    }
    stage('Build and Deploy') {
        dir("source") {
            sh "mkdir -p build-folder/target/ build-folder/app/"
            sh "cp ocp.Dockerfile build-folder/Dockerfile"
            sh "cp ciam/ciam.csproj build-folder"

            def tag = sh(returnStdout: true, script: "git rev-parse --short=8 HEAD").trim();

            sh "cat build-folder/Dockerfile | oc new-build -D - --name ${appName} || true"
            sh "oc start-build ${appName} --from-dir=build-folder/. --follow --wait "
            
            sh "oc tag cicd/${appName}:latest ${projectName}/${appName}:${tag} "
            sh "oc tag cicd/${appName}:latest ${projectName}/${appName}:latest "


            sh "sed 's,\\\$REGISTRY/\\\$HARBOR_NAMESPACE/\\\$APP_NAME:\\\$BUILD_NUMBER,image-registry.openshift-image-registry.svc:5000/${projectName}/${appName}:latest,g' ocp-kubernetes.yaml > kubernetes-ocp.yaml"
            sh "oc apply -f kubernetes-ocp.yaml -n ${projectName}"
            sh "oc set triggers deployment/${appName} --from-image=${projectName}/${appName}:latest -c ${appName} -n ${projectName} || true "
        }
    }
}
