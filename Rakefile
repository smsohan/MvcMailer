require 'rexml/document'
include REXML

PROJECT_NAME = "Mvc.Mailer"
BUILD_CONFIG = "Debug"

def build target_name, target, build_config
	msbuild_path = "C:/Windows/Microsoft.NET/Framework/v4.0.30319/msbuild.exe"
	config = "#{target_name} /p:Configuration=#{build_config} /t:#{target} /nologo /verbosity:minimal"
	sh "#{msbuild_path} #{config}"
end

def test test_project_name
	nunit_path = "packages/NUnit.Runners.2.6.1/tools/nunit-console.exe"
	config = "#{test_project_name}/bin/#{BUILD_CONFIG}/#{test_project_name}.dll /xml=build/#{test_project_name}.xml /nologo"
	sh "#{nunit_path} #{config}"
end

def update_nuspec_version version
	#package//metadata/version
	config = nil
	File.open("#{PROJECT_NAME}.nuspec", 'r') do |config_file|
		config = Document.new(config_file)
		node = config.root.elements['metadata/version']
		node.text = version.to_s
	end
	
	formatter = REXML::Formatters::Default.new
	File.open("#{PROJECT_NAME}.nuspec", 'w') do |result|
		formatter.write(config, result)
	end
end

task :default => [:clean, :compile, :test]

task :clean do
	build "#{PROJECT_NAME}.sln", "clean", BUILD_CONFIG
end

task :compile => [:clean] do
	build "#{PROJECT_NAME}.sln", "build", BUILD_CONFIG
end

task :test => [:compile] do
	test "#{PROJECT_NAME}.Test"
end

#rake nuget v=0.3.0
task :package do
	if ENV['v']
		puts "Updating NuGet package to version #{ENV['v']}"
		update_nuspec_version "#{ENV['v']}"
	end
	build "#{PROJECT_NAME}.sln", "build", "NuGet"
	sh "nuget pack #{PROJECT_NAME}.nuspec -b build\\nuget -o build\\nuget_packages"
end

#rake nuget v=0.3.0 k=<nuget_access_key>
task :publish => [:package] do
	if ENV['k'] && ENV['v']
		sh "nuget push build\\nuget_packages\\#{PROJECT_NAME}#{ENV['v']}.nupkg #{ENV['k']}"
	end
end