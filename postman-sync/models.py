class TestRequirementDto:
    def __init__(self, name, key, expected):
        self.name = name
        self.key = key
        self.expected = expected

class TestDto:
    def __init__(self, name, requirements=[], request_body={}):
        self.name = name
        self.requirements = requirements
        self.request_body = request_body

