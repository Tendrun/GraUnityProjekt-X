package dev.kjk_kjp.projektx;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
//import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseBody;


@SpringBootApplication
@Controller
public class ProjektxApplication {

	public static void main(String[] args) {
		SpringApplication.run(ProjektxApplication.class, args);
	}

	@GetMapping(value="/hello")
	@ResponseBody
	public String hello() {
		return "Hello World";
	}
	
}
