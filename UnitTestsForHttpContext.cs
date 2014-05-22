            // Setup Controller
            _controller = new PlayChipStoreController(_mapper, _session, _paymentService, _middleware, _marketingService,
                                                      _geoAddressService, _fieldBuilder, _cashier)
                              {
                                  ControllerContext = new ControllerContext
                                                          {
                                                              HttpContext = FakeHttpContext.Generic(),
                                                              RequestContext = new RequestContext(),
                                                              RouteData = new RouteData()
                                                          },
                                  ValueProvider = new FormCollection() { /*{ "Some key", "Some Value" }, */}.ToValueProvider()
                              };